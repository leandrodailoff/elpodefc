namespace ElPodeFC.Api.Domain;

/// <summary>Foto, video o embebido (YouTube). Los archivos NO viven en la BD, solo la ruta/URL.</summary>
public class Medio
{
    public long Id { get; set; }
    public string Tipo { get; set; } = "imagen";   // imagen | video | youtube
    public string? ArchivoRuta { get; set; }
    public string? Url { get; set; }
    public long? PartidoId { get; set; }
    public string? Titulo { get; set; }
    public string? SubidoPor { get; set; }
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;

    public Partido? Partido { get; set; }
}