namespace ElPodeFC.Api.Dtos;

/// <summary>Partido como aparece en el historial (sin las actuaciones).</summary>
public record PartidoDto(
    long Id,
    DateTimeOffset Fecha,
    string? Rival,
    string Resultado,
    short MarcadorPropio,
    short MarcadorRival,
    string? Tipo,
    string? Notas,
    string? RegistradoPor,
    int CantidadMedios);

/// <summary>Una actuación dentro del detalle del partido.</summary>
public record ActuacionDto(
    long Id,
    long JugadorId,
    string Jugador,
    string? Posicion,
    short Goles,
    bool Mvp);

/// <summary>Detalle completo: datos + actuaciones + grabaciones del partido.</summary>
public record PartidoDetalleDto(
    long Id,
    DateTimeOffset Fecha,
    string? Rival,
    string Resultado,
    short MarcadorPropio,
    short MarcadorRival,
    string? Tipo,
    string? Notas,
    string? RegistradoPor,
    List<ActuacionDto> Actuaciones,
    List<MedioDto> Medios);

/// <summary>Actuación que llega en el payload de alta/edición de un partido.</summary>
public record ActuacionGuardarDto(
    long JugadorId,
    string? Posicion,
    short Goles,
    bool Mvp);

/// <summary>
/// Alta o corrección de un partido. <b>Un solo POST crea el partido completo</b>
/// (datos + actuaciones): es la base del flujo "&lt; 2 minutos".
/// </summary>
public record PartidoGuardarDto(
    DateTimeOffset Fecha,
    string? Rival,
    string? Resultado,
    short MarcadorPropio,
    short MarcadorRival,
    string? Tipo,
    string? Notas,
    string? RegistradoPor,
    List<ActuacionGuardarDto>? Actuaciones);