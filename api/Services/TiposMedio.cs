namespace ElPodeFC.Api.Services;

/// <summary>Qué es cada medio: lo decide el tipo de archivo o el link, nunca el cliente.</summary>
public static class TiposMedio
{
    public static readonly string[] ExtensionesImagen =
        [".jpg", ".jpeg", ".png", ".webp", ".gif"];

    public static readonly string[] ExtensionesVideo =
        [".mp4", ".webm", ".mov", ".mkv"];

    public static bool EsImagen(string nombreArchivo) =>
        ExtensionesImagen.Contains(Path.GetExtension(nombreArchivo).ToLowerInvariant());

    public static bool EsVideo(string nombreArchivo) =>
        ExtensionesVideo.Contains(Path.GetExtension(nombreArchivo).ToLowerInvariant());

    /// <summary>¿El link es de YouTube? (se muestra embebido, no descargado).</summary>
    public static bool EsYoutube(string url) =>
        url.Contains("youtube.com", StringComparison.OrdinalIgnoreCase) ||
        url.Contains("youtu.be", StringComparison.OrdinalIgnoreCase);
}