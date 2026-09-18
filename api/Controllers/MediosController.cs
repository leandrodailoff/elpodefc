using ElPodeFC.Api.Auth;
using ElPodeFC.Api.Data;
using ElPodeFC.Api.Domain;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Mapping;
using ElPodeFC.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElPodeFC.Api.Controllers;

/// <summary>Formulario de subida (multipart/form-data): un archivo O un link.</summary>
public class MedioSubidaForm
{
    public IFormFile? Archivo { get; set; }
    public string? Url { get; set; }
    public long? PartidoId { get; set; }
    public string? Titulo { get; set; }
    public string? SubidoPor { get; set; }
}

/// <summary>
/// Fotos y grabaciones. El archivo va al disco (volumen <c>/data/medios</c>) y en la BD
/// queda solo la ruta: es lo que mantiene la base chica para siempre.
/// </summary>
[ApiController]
[Route("api/medios")]
public class MediosController(ElPodeFCContext db, IAlmacenMedios almacen) : ControllerBase
{
    /// <summary>
    /// Galería general, o las <b>grabaciones de un partido</b> con <c>?partido_id=42</c>.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MedioDto>>> Listar(
        [FromQuery(Name = "partido_id")] long? partidoId,
        [FromQuery] string? tipo,
        [FromQuery] int? limit)
    {
        var consulta = db.Medios.AsNoTracking().AsQueryable();

        if (partidoId is not null)
            consulta = consulta.Where(m => m.PartidoId == partidoId);

        if (!string.IsNullOrWhiteSpace(tipo))
        {
            var t = tipo.Trim().ToLowerInvariant();
            consulta = consulta.Where(m => m.Tipo == t);
        }

        var tope = Math.Clamp(limit ?? 200, 1, 500);

        var medios = await consulta
            .OrderByDescending(m => m.CreadoEn)
            .ThenByDescending(m => m.Id)
            .Take(tope)
            .ToListAsync();

        return medios.Select(m => m.ADto()).ToList();
    }

    /// <summary>Sube una foto/video (archivo) o registra un link (por ejemplo, de YouTube).</summary>
    [HttpPost]
    [RequiereClave]
    [RequestSizeLimit(500L * 1024 * 1024)]
    public async Task<ActionResult<MedioDto>> Subir([FromForm] MedioSubidaForm form)
    {
        if (form.Archivo is null && string.IsNullOrWhiteSpace(form.Url))
            return BadRequest(new { error = "Mandá un archivo o una URL." });

        if (form.Archivo is not null && !string.IsNullOrWhiteSpace(form.Url))
            return BadRequest(new { error = "Es un archivo O una URL, no las dos cosas." });

        if (form.PartidoId is not null &&
            !await db.Partidos.AnyAsync(p => p.Id == form.PartidoId))
            return BadRequest(new { error = $"No existe el partido {form.PartidoId}." });

        string tipo;
        string? archivoRuta = null;
        string? url = null;

        if (form.Archivo is not null)
        {
            tipo = TiposMedio.EsImagen(form.Archivo.FileName) ? "imagen" : "video";

            try
            {
                archivoRuta = await almacen.GuardarAsync(form.Archivo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        else
        {
            url = form.Url!.Trim();
            tipo = TiposMedio.EsYoutube(url) ? "youtube" : "video";
        }

        var medio = new Medio
        {
            Tipo = tipo,
            ArchivoRuta = archivoRuta,
            Url = url,
            PartidoId = form.PartidoId,
            Titulo = Limpiar(form.Titulo),
            SubidoPor = Limpiar(form.SubidoPor),
        };

        db.Medios.Add(medio);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = medio.Id }, medio.ADto());
    }

    /// <summary>Borra el medio: también el archivo del disco, si lo tiene.</summary>
    [HttpDelete("{id:long}")]
    [RequiereClave]
    public async Task<IActionResult> Borrar(long id)
    {
        var medio = await db.Medios.FirstOrDefaultAsync(m => m.Id == id);
        if (medio is null) return NotFound(new { error = $"No existe el medio {id}." });

        db.Medios.Remove(medio);
        await db.SaveChangesAsync();

        // El archivo se borra después de que la BD confirmó: nunca al revés.
        almacen.Borrar(medio.ArchivoRuta);

        return NoContent();
    }

    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}