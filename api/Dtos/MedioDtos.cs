namespace ElPodeFC.Api.Dtos;

/// <summary>
/// Foto, video o embebido. <c>Url</c> es siempre lo que consume el frontend:
/// un link de YouTube, o <c>/medios/{archivo}</c> cuando el archivo está en el servidor.
/// </summary>
public record MedioDto(
    long Id,
    string Tipo,
    string? Url,
    string? ArchivoRuta,
    long? PartidoId,
    string? Titulo,
    string? SubidoPor,
    DateTimeOffset CreadoEn);