using ElPodeFC.Api.Auth;
using ElPodeFC.Api.Data;
using ElPodeFC.Api.Domain;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElPodeFC.Api.Controllers;

/// <summary>Plantel: lista, perfil con stats calculadas y ABM (con clave).</summary>
[ApiController]
[Route("api/jugadores")]
public class JugadoresController(ElPodeFCContext db) : ControllerBase
{
    /// <summary>Lista del plantel. <c>?activo=true|false</c> filtra por presente/histórico.</summary>
    [HttpGet]
    public async Task<ActionResult<List<JugadorDto>>> Listar([FromQuery] bool? activo)
    {
        var consulta = db.Jugadores.AsNoTracking().AsQueryable();

        if (activo is not null)
            consulta = consulta.Where(j => j.Activo == activo);

        var jugadores = await consulta
            .OrderByDescending(j => j.Activo)
            .ThenBy(j => j.Nombre)
            .ToListAsync();

        return jugadores.Select(j => j.ADto()).ToList();
    }

    /// <summary>Perfil del jugador con sus estadísticas derivadas de las actuaciones.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<JugadorPerfilDto>> Obtener(long id)
    {
        var jugador = await db.Jugadores.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
        if (jugador is null) return NotFound(new { error = $"No existe el jugador {id}." });

        var actuaciones = await db.Actuaciones
            .AsNoTracking()
            .Where(a => a.JugadorId == id)
            .Select(a => new { a.Goles, a.Mvp, a.Partido.Resultado })
            .ToListAsync();

        var partidos = actuaciones.Count;

        var stats = new JugadorStatsDto(
            Partidos: partidos,
            Ganados: actuaciones.Count(a => a.Resultado == "G"),
            Empatados: actuaciones.Count(a => a.Resultado == "E"),
            Perdidos: actuaciones.Count(a => a.Resultado == "P"),
            Goles: actuaciones.Sum(a => (int)a.Goles),
            Mvp: actuaciones.Count(a => a.Mvp),
            GolesPorPartido: partidos == 0
                ? 0
                : Math.Round(actuaciones.Sum(a => (int)a.Goles) / (double)partidos, 2));

        return new JugadorPerfilDto(
            jugador.Id, jugador.Nombre, jugador.Gamertag, jugador.PosicionHabitual,
            jugador.FotoUrl, jugador.Activo, stats);
    }

    /// <summary>Alta de jugador.</summary>
    [HttpPost]
    [RequiereClave]
    public async Task<ActionResult<JugadorDto>> Crear([FromBody] JugadorGuardarDto datos)
    {
        var error = Validar(datos);
        if (error is not null) return BadRequest(new { error });

        var jugador = new Jugador
        {
            Nombre = datos.Nombre.Trim(),
            Gamertag = Limpiar(datos.Gamertag),
            PosicionHabitual = Limpiar(datos.PosicionHabitual),
            FotoUrl = Limpiar(datos.FotoUrl),
            Activo = datos.Activo ?? true,
            CreadoEn = DateTimeOffset.UtcNow,
            ActualizadoEn = DateTimeOffset.UtcNow,
        };

        db.Jugadores.Add(jugador);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = jugador.Id }, jugador.ADto());
    }

    /// <summary>Edición de jugador (nombre, gamertag, posición, foto, activo).</summary>
    [HttpPut("{id:long}")]
    [RequiereClave]
    public async Task<ActionResult<JugadorDto>> Editar(long id, [FromBody] JugadorGuardarDto datos)
    {
        var error = Validar(datos);
        if (error is not null) return BadRequest(new { error });

        var jugador = await db.Jugadores.FirstOrDefaultAsync(j => j.Id == id);
        if (jugador is null) return NotFound(new { error = $"No existe el jugador {id}." });

        jugador.Nombre = datos.Nombre.Trim();
        jugador.Gamertag = Limpiar(datos.Gamertag);
        jugador.PosicionHabitual = Limpiar(datos.PosicionHabitual);
        jugador.FotoUrl = Limpiar(datos.FotoUrl);
        jugador.Activo = datos.Activo ?? jugador.Activo;
        jugador.ActualizadoEn = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();
        return jugador.ADto();
    }

    /// <summary>Baja del jugador: borra su historial de actuaciones en cascada.</summary>
    [HttpDelete("{id:long}")]
    [RequiereClave]
    public async Task<IActionResult> Borrar(long id)
    {
        var jugador = await db.Jugadores.FirstOrDefaultAsync(j => j.Id == id);
        if (jugador is null) return NotFound(new { error = $"No existe el jugador {id}." });

        db.Jugadores.Remove(jugador);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static string? Validar(JugadorGuardarDto datos) =>
        string.IsNullOrWhiteSpace(datos.Nombre) ? "El nombre es obligatorio." : null;

    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}