namespace ElPodeFC.Api.Services;

/// <summary>
/// Guarda los archivos de medios FUERA de la base (ver `esquema-bd.md`: la BD solo
/// guarda la ruta). En la VPS el volumen apunta a <c>/data/medios</c>.
/// </summary>
public interface IAlmacenMedios
{
    /// <summary>Guarda el archivo y devuelve el nombre relativo (lo que se persiste).</summary>
    Task<string> GuardarAsync(IFormFile archivo, CancellationToken ct = default);

    /// <summary>Borra el archivo del disco. Silencioso si ya no existe.</summary>
    void Borrar(string? rutaRelativa);

    /// <summary>Ruta pública con la que el frontend pide el archivo.</summary>
    static string Url(string rutaRelativa) => $"/medios/{rutaRelativa}";
}

/// <summary>Implementación sobre el filesystem local.</summary>
public sealed class AlmacenMedios : IAlmacenMedios
{
    /// <summary>Límites propuestos en `api-endpoints.md`.</summary>
    private const long MaxImagen = 10L * 1024 * 1024;        //  10 MB
    private const long MaxVideo = 500L * 1024 * 1024;        // 500 MB

    private readonly string _raiz;
    private readonly ILogger<AlmacenMedios> _log;

    public AlmacenMedios(IConfiguration config, IHostEnvironment entorno, ILogger<AlmacenMedios> log)
    {
        _log = log;
        var configurada = config["Almacen:Raiz"] ?? "data/medios";
        _raiz = Path.IsPathRooted(configurada)
            ? configurada
            : Path.Combine(entorno.ContentRootPath, configurada);
        Directory.CreateDirectory(_raiz);
    }

    public async Task<string> GuardarAsync(IFormFile archivo, CancellationToken ct = default)
    {
        if (archivo.Length == 0)
            throw new ArgumentException("El archivo está vacío.");

        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
        var esImagen = TiposMedio.EsImagen(archivo.FileName);
        var esVideo = TiposMedio.EsVideo(archivo.FileName);

        if (!esImagen && !esVideo)
            throw new ArgumentException(
                $"Extensión no permitida: '{extension}'. " +
                $"Imágenes: {string.Join(", ", TiposMedio.ExtensionesImagen)} · " +
                $"Videos: {string.Join(", ", TiposMedio.ExtensionesVideo)}");

        var limite = esImagen ? MaxImagen : MaxVideo;
        if (archivo.Length > limite)
            throw new ArgumentException($"El archivo supera el máximo de {limite / 1024 / 1024} MB.");

        // Nombre único: nunca se pisa un archivo existente ni se confía en el nombre original.
        var nombre = $"{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}{extension}";
        var destino = Path.Combine(_raiz, nombre);

        await using var stream = File.Create(destino);
        await archivo.CopyToAsync(stream, ct);

        _log.LogInformation("Medio guardado: {Nombre} ({Bytes} bytes)", nombre, archivo.Length);
        return nombre;
    }

    public void Borrar(string? rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa)) return;

        // Solo el nombre del archivo: evita que un valor raro se escape de la carpeta.
        var nombre = Path.GetFileName(rutaRelativa);
        var destino = Path.Combine(_raiz, nombre);

        if (File.Exists(destino))
        {
            File.Delete(destino);
            _log.LogInformation("Medio borrado: {Nombre}", nombre);
        }
    }
}