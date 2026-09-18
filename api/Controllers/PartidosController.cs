using ElPodeFC.Api.Auth;
using ElPodeFC.Api.Data;
using ElPodeFC.Api.Domain;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElPodeFC.Api.Controllers;

/// <summary>
/// Historial y detalle de partidos. <b>El POST crea el partido completo en una llamada</b>
/// (datos + actuaciones) y en una transacción: o entra todo, o no entra nada.
/// </summary>
[ApiController]
[Route("api/partidos")]
public class PartidosController(ElPodeFCContext db) : ControllerBase
{
    private const int LimitePorDefecto = 100;
    private const int LimiteMaximo = 500;

    /// <summary>
    /// Historial. Filtros: <c>?resultado=G&amp;tipo=Liga&amp;desde=2026-01-01&amp;hasta=2026-12-31&amp;limit=50</c>
    /// (sin paginación real en v1: alcanza para la escala de un club).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PartidoDto>>> Listar(
        [FromQuery] string? resultado,
        [FromQuery] string? tipo,
        [FromQuery] DateTimeOffset? desde,
        [FromQuery] DateTimeOffset? hasta,
        [FromQuery] int? limit)
    {
        var consulta = db.Partidos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(resultado))
        {
            var letra = resultado.Trim().ToUpperInvariant();
            if (letra is not ("G" or "E" or "P"))
                return BadRequest(new { error = "El filtro 'resultado' debe ser G, E o P." });

            consulta = consulta.Where(p => p.Resultado == letra);
        }

        if (!string.IsNullOrWhiteSpace(tipo))
            consulta = consulta.Where(p => p.Tipo == tipo.Trim());

        // A UTC: Npgsql no acepta offsets distintos de 0 en parámetros de 'timestamptz'.
        if (desde is not null) consulta = consulta.Where(p => p.Fecha >= desde.Value.ToUniversalTime());
        if (hasta is not null) consulta = consulta.Where(p => p.Fecha <= hasta.Value.ToUniversalTime());

        var tope = Math.Clamp(limit ?? LimitePorDefecto, 1, LimiteMaximo);

        var partidos = await consulta
            .OrderByDescending(p => p.Fecha)
            .ThenByDescending(p => p.Id)
            .Take(tope)
            .Select(p => new { Partido = p, Medios = p.Medios.Count })
            .ToListAsync();

        return partidos.Select(x => x.Partido.ADto(x.Medios)).ToList();
    }

    /// <summary>Detalle completo: actuaciones + las grabaciones del partido (por id).</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<PartidoDetalleDto>> Obtener(long id)
    {
        var detalle = await ArmarDetalle(id);
        return detalle is null ? NotFound(new { error = $"No existe el partido {id}." }) : detalle;
    }

    /// <summary>Alta del partido completo (el flujo de carga de la noche, en &lt; 2 minutos).</summary>
    [HttpPost]
    [RequiereClave]
    public async Task<ActionResult<PartidoDetalleDto>> Crear([FromBody] PartidoGuardarDto datos)
    {
        var errorValidacion = await ValidarAsync(datos);
        if (errorValidacion is not null) return BadRequest(new { error = errorValidacion });

        await using var tx = await db.Database.BeginTransactionAsync();

        var partido = new Partido
        {
            Fecha = datos.Fecha,
            Rival = Limpiar(datos.Rival),
            Resultado = Resultado(datos)!,
            MarcadorPropio = datos.MarcadorPropio,
            MarcadorRival = datos.MarcadorRival,
            Tipo = Limpiar(datos.Tipo),
            Notas = Limpiar(datos.Notas),
            RegistradoPor = Limpiar(datos.RegistradoPor),
        };

        partido.Actuaciones = [.. (datos.Actuaciones ?? []).Select(a => new Actuacion
        {
            JugadorId = a.JugadorId,
            Posicion = Limpiar(a.Posicion),
            Goles = a.Goles,
            Mvp = a.Mvp,
        })];

        db.Partidos.Add(partido);
        await db.SaveChangesAsync();     // una sola escritura = todo o nada
        await tx.CommitAsync();

        return CreatedAtAction(nameof(Obtener), new { id = partido.Id }, await ArmarDetalle(partido.Id));
    }

    /// <summary>Corrige un partido ya cargado (incluye reemplazar sus actuaciones).</summary>
    [HttpPut("{id:long}")]
    [RequiereClave]
    public async Task<ActionResult<PartidoDetalleDto>> Editar(long id, [FromBody] PartidoGuardarDto datos)
    {
        var errorValidacion = await ValidarAsync(datos);
        if (errorValidacion is not null) return BadRequest(new { error = errorValidacion });

        var partido = await db.Partidos
            .Include(p => p.Actuaciones)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partido is null) return NotFound(new { error = $"No existe el partido {id}." });

        await using var tx = await db.Database.BeginTransactionAsync();

        partido.Fecha = datos.Fecha;
        partido.Rival = Limpiar(datos.Rival);
        partido.Resultado = Resultado(datos)!;
        partido.MarcadorPropio = datos.MarcadorPropio;
        partido.MarcadorRival = datos.MarcadorRival;
        partido.Tipo = Limpiar(datos.Tipo);
        partido.Notas = Limpiar(datos.Notas);
        partido.RegistradoPor = Limpiar(datos.RegistradoPor);
        partido.ActualizadoEn = DateTimeOffset.UtcNow;

        // Las actuaciones se reemplazan enteras: es más simple de razonar que un diff.
        db.Actuaciones.RemoveRange(partido.Actuaciones);
        partido.Actuaciones = [.. (datos.Actuaciones ?? []).Select(a => new Actuacion
        {
            PartidoId = partido.Id,
            JugadorId = a.JugadorId,
            Posicion = Limpiar(a.Posicion),
            Goles = a.Goles,
            Mvp = a.Mvp,
        })];

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return (await ArmarDetalle(partido.Id))!;
    }

    /// <summary>Borra el partido: las actuaciones se van en cascada, los medios se sueltan.</summary>
    [HttpDelete("{id:long}")]
    [RequiereClave]
    public async Task<IActionResult> Borrar(long id)
    {
        var partido = await db.Partidos.FirstOrDefaultAsync(p => p.Id == id);
        if (partido is null) return NotFound(new { error = $"No existe el partido {id}." });

        db.Partidos.Remove(partido);
        await db.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>Arma el detalle completo (lo usan el GET y las respuestas de escritura).</summary>
    private async Task<PartidoDetalleDto?> ArmarDetalle(long id)
    {
        var partido = await db.Partidos
            .AsNoTracking()
            .Include(p => p.Actuaciones).ThenInclude(a => a.Jugador)
            .Include(p => p.Medios)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (partido is null) return null;

        return new PartidoDetalleDto(
            partido.Id, partido.Fecha, partido.Rival, partido.Resultado,
            partido.MarcadorPropio, partido.MarcadorRival, partido.Tipo,
            partido.Notas, partido.RegistradoPor,
            [.. partido.Actuaciones
                .OrderByDescending(a => a.Goles)
                .ThenBy(a => a.Jugador.Nombre)
                .Select(a => a.ADto())],
            [.. partido.Medios.OrderByDescending(m => m.CreadoEn).Select(m => m.ADto())]);
    }

    /// <summary>Devuelve el mensaje de error, o null si el payload está bien.</summary>
    private async Task<string?> ValidarAsync(PartidoGuardarDto datos)
    {
        if (datos.Fecha == default)
            return "La fecha del partido es obligatoria.";

        if (datos.MarcadorPropio < 0 || datos.MarcadorRival < 0)
            return "Los marcadores no pueden ser negativos.";

        if (Resultado(datos) is null)
            return "El resultado debe ser G, E o P.";

        var actuaciones = datos.Actuaciones ?? [];

        if (actuaciones.Any(a => a.Goles < 0))
            return "Los goles no pueden ser negativos.";

        var ids = actuaciones.Select(a => a.JugadorId).ToList();

        if (ids.Any(id => id <= 0))
            return "Hay una actuación sin jugador.";

        if (ids.Distinct().Count() != ids.Count)
            return "Hay jugadores repetidos en las actuaciones (un jugador juega una vez por partido).";

        if (ids.Count > 0)
        {
            var existentes = await db.Jugadores
                .Where(j => ids.Contains(j.Id))
                .Select(j => j.Id)
                .ToListAsync();

            var faltantes = ids.Except(existentes).ToList();
            if (faltantes.Count > 0)
                return $"No existen los jugadores: {string.Join(", ", faltantes)}.";
        }

        return null;
    }

    /// <summary>
    /// El resultado que se guarda: el que manda el frontend si es válido, o el que
    /// cantan los marcadores. Devuelve null si el que llegó no es G/E/P.
    /// </summary>
    private static string? Resultado(PartidoGuardarDto datos)
    {
        if (!string.IsNullOrWhiteSpace(datos.Resultado))
        {
            var letra = datos.Resultado.Trim().ToUpperInvariant();
            return letra is "G" or "E" or "P" ? letra : null;
        }

        return datos.MarcadorPropio > datos.MarcadorRival ? "G"
             : datos.MarcadorPropio < datos.MarcadorRival ? "P"
             : "E";
    }

    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}