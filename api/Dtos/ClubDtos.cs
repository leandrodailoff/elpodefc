namespace ElPodeFC.Api.Dtos;

/// <summary>
/// Ficha del club que alimenta el widget del Inicio.
/// <c>JuegaHoy</c> lo calcula el SERVIDOR con su zona horaria (el teléfono no opina).
/// </summary>
public record ClubDto(
    string? DiscordUrl,
    TimeOnly? HorarioJuego,
    string[] DiasJuego,
    bool Pausado,
    bool JuegaHoy,
    string DiaDeHoy);

/// <summary>Edición de la ficha del club (horario, días y Discord).</summary>
public record ClubGuardarDto(
    string? DiscordUrl,
    TimeOnly? HorarioJuego,
    string[]? DiasJuego,
    bool? Pausado);