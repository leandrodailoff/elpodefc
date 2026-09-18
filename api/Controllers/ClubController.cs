using ElPodeFC.Api.Auth;
using ElPodeFC.Api.Data;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElPodeFC.Api.Controllers;

/// <summary>Ficha del club (horario, días, Discord) y el panel de estadísticas.</summary>
[ApiController]
[Route("api/club")]
public class ClubController(ElPodeFCContext db, RelojClub reloj) : ControllerBase
{
    /// <summary>Datos del club + <c>juega_hoy</c> calculado por el servidor.</summary>
    [HttpGet]
    public async Task<ActionResult<ClubDto>> Obtener()
    {
        var club = await db.ClubConfig.AsNoTracking().FirstOrDefaultAsync();
        if (club is null) return NotFound(new { error = "El club no tiene ficha configurada." });

        return new ClubDto(
            club.DiscordUrl,
            club.HorarioJuego,
            club.DiasJuego,
            club.Pausado,
            reloj.JuegaHoy(club.DiasJuego, club.Pausado),
            reloj.DiaDeHoy);
    }

    /// <summary>Actualiza la ficha del club (lo que se toca en <c>/admin/club</c>).</summary>
    [HttpPut]
    [RequiereClave]
    public async Task<ActionResult<ClubDto>> Editar([FromBody] ClubGuardarDto datos)
    {
        var club = await db.ClubConfig.FirstOrDefaultAsync();
        if (club is null) return NotFound(new { error = "El club no tiene ficha configurada." });

        var dias = new List<string>();
        foreach (var dia in datos.DiasJuego ?? [])
        {
            var normalizado = RelojClub.NormalizarDia(dia);
            if (normalizado is null)
                return BadRequest(new { error = $"Día inválido: '{dia}'. Vale dom|Lun|mar|mie|jue|vie|sab." });

            if (!dias.Contains(normalizado)) dias.Add(normalizado);
        }

        club.DiscordUrl = string.IsNullOrWhiteSpace(datos.DiscordUrl) ? null : datos.DiscordUrl.Trim();
        club.HorarioJuego = datos.HorarioJuego;
        club.DiasJuego = [.. dias.OrderBy(d => Array.IndexOf(RelojClub.Dias, d))];
        club.Pausado = datos.Pausado ?? club.Pausado;
        club.ActualizadoEn = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        return new ClubDto(
            club.DiscordUrl, club.HorarioJuego, club.DiasJuego, club.Pausado,
            reloj.JuegaHoy(club.DiasJuego, club.Pausado), reloj.DiaDeHoy);
    }

    /// <summary>
    /// Panel del club. TODO sale de una consulta: no hay un solo número guardado.
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasDto>> Estadisticas()
    {
        var partidos = await db.Partidos
            .AsNoTracking()
            .OrderByDescending(p => p.Fecha)
            .ThenByDescending(p => p.Id)
            .Select(p => new { p.Resultado, p.MarcadorPropio, p.MarcadorRival })
            .ToListAsync();

        // Se agrupa y ordena en SQL con un tipo anónimo: ordenar un `record` proyectado
        // no lo traduce EF. El paso a DTO se hace en memoria, con 10 filas.
        var goleadoresSql = await db.Actuaciones
            .AsNoTracking()
            .Where(a => a.Goles > 0)
            .GroupBy(a => new { a.JugadorId, a.Jugador.Nombre })
            .Select(g => new { g.Key.JugadorId, g.Key.Nombre, Cantidad = g.Sum(a => (int)a.Goles) })
            .OrderByDescending(g => g.Cantidad)
            .ThenBy(g => g.Nombre)
            .Take(10)
            .ToListAsync();

        var mvpsSql = await db.Actuaciones
            .AsNoTracking()
            .Where(a => a.Mvp)
            .GroupBy(a => new { a.JugadorId, a.Jugador.Nombre })
            .Select(g => new { g.Key.JugadorId, g.Key.Nombre, Cantidad = g.Count() })
            .OrderByDescending(g => g.Cantidad)
            .ThenBy(g => g.Nombre)
            .Take(10)
            .ToListAsync();

        var goleadores = goleadoresSql
            .Select(g => new RankingDto(g.JugadorId, g.Nombre, g.Cantidad))
            .ToList();

        var mvps = mvpsSql
            .Select(g => new RankingDto(g.JugadorId, g.Nombre, g.Cantidad))
            .ToList();

        return new EstadisticasDto(
            Partidos: partidos.Count,
            Ganados: partidos.Count(p => p.Resultado == "G"),
            Empatados: partidos.Count(p => p.Resultado == "E"),
            Perdidos: partidos.Count(p => p.Resultado == "P"),
            GolesPropios: partidos.Sum(p => (int)p.MarcadorPropio),
            GolesRivales: partidos.Sum(p => (int)p.MarcadorRival),
            Racha: Racha(partidos.Select(p => p.Resultado)),
            Goleadores: goleadores,
            Mvps: mvps,
            UltimosResultados: [.. partidos.Take(10).Select(p => p.Resultado)]);
    }

    /// <summary>Cuenta cuántos resultados iguales hay al final del historial (más reciente primero).</summary>
    private static RachaDto Racha(IEnumerable<string> resultados)
    {
        var lista = resultados.ToList();
        if (lista.Count == 0) return new RachaDto("", 0);

        var primero = lista[0];
        return new RachaDto(primero, lista.TakeWhile(r => r == primero).Count());
    }
}