using ElPodeFC.Api.Auth;
using ElPodeFC.Api.Data;
using ElPodeFC.Api.Domain;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElPodeFC.Api.Controllers;

/// <summary>
/// La "canchita": esquemas de formación (11 titular). El dibujo se guarda como JSONB,
/// así que mover un jugador de puesto no necesita migración.
/// </summary>
[ApiController]
[Route("api/formaciones")]
public class FormacionesController(ElPodeFCContext db) : ControllerBase
{
    /// <summary>Lista de esquemas (sin el detalle del dibujo, que es pesado).</summary>
    [HttpGet]
    public async Task<ActionResult<List<FormacionDto>>> Listar()
    {
        var formaciones = await db.Formaciones
            .AsNoTracking()
            .OrderByDescending(f => f.CreadoEn)
            .ToListAsync();

        return formaciones.Select(f => f.ADto()).ToList();
    }

    /// <summary>Un esquema en detalle: el JSONB con posiciones y jugadores.</summary>
    [HttpGet("{id:long}")]
    public async Task<ActionResult<FormacionDto>> Obtener(long id)
    {
        var formacion = await db.Formaciones.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        return formacion is null
            ? NotFound(new { error = $"No existe la formación {id}." })
            : formacion.ADto();
    }

    /// <summary>Crea un esquema (por ejemplo, "11 titular 2026").</summary>
    [HttpPost]
    [RequiereClave]
    public async Task<ActionResult<FormacionDto>> Crear([FromBody] FormacionGuardarDto datos)
    {
        var error = Validar(datos);
        if (error is not null) return BadRequest(new { error });

        var formacion = new Formacion
        {
            Nombre = datos.Nombre.Trim(),
            Esquema = datos.Esquema.GetRawText(),
        };

        db.Formaciones.Add(formacion);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Obtener), new { id = formacion.Id }, formacion.ADto());
    }

    /// <summary>Mueve jugadores en la cancha: reemplaza el esquema entero.</summary>
    [HttpPut("{id:long}")]
    [RequiereClave]
    public async Task<ActionResult<FormacionDto>> Editar(long id, [FromBody] FormacionGuardarDto datos)
    {
        var error = Validar(datos);
        if (error is not null) return BadRequest(new { error });

        var formacion = await db.Formaciones.FirstOrDefaultAsync(f => f.Id == id);
        if (formacion is null) return NotFound(new { error = $"No existe la formación {id}." });

        formacion.Nombre = datos.Nombre.Trim();
        formacion.Esquema = datos.Esquema.GetRawText();

        await db.SaveChangesAsync();
        return formacion.ADto();
    }

    /// <summary>Borra el esquema.</summary>
    [HttpDelete("{id:long}")]
    [RequiereClave]
    public async Task<IActionResult> Borrar(long id)
    {
        var formacion = await db.Formaciones.FirstOrDefaultAsync(f => f.Id == id);
        if (formacion is null) return NotFound(new { error = $"No existe la formación {id}." });

        db.Formaciones.Remove(formacion);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static string? Validar(FormacionGuardarDto datos)
    {
        if (string.IsNullOrWhiteSpace(datos.Nombre))
            return "El nombre de la formación es obligatorio.";

        if (datos.Esquema.ValueKind != System.Text.Json.JsonValueKind.Object)
            return "El esquema debe ser un objeto JSON, por ejemplo { \"titulares\": [...] }.";

        return null;
    }
}