namespace ElPodeFC.Api.Dtos;

/// <summary>Un jugador en un ranking (goles o MVP).</summary>
public record RankingDto(
    long JugadorId,
    string Jugador,
    int Cantidad);

/// <summary>La racha actual del club (misma letra repetida al final del historial).</summary>
public record RachaDto(
    string Tipo,
    int Cantidad);

/// <summary>Panel de estadísticas del club: todo derivado de los partidos y actuaciones.</summary>
public record EstadisticasDto(
    int Partidos,
    int Ganados,
    int Empatados,
    int Perdidos,
    int GolesPropios,
    int GolesRivales,
    RachaDto Racha,
    List<RankingDto> Goleadores,
    List<RankingDto> Mvps,
    List<string> UltimosResultados);