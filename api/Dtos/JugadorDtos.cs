namespace ElPodeFC.Api.Dtos;

/// <summary>Jugador del plantel (item de la lista).</summary>
public record JugadorDto(
    long Id,
    string Nombre,
    string? Gamertag,
    string? PosicionHabitual,
    string? FotoUrl,
    bool Activo);

/// <summary>Stats derivadas: NUNCA se guardan, salen de las actuaciones (ver esquema-bd.md).</summary>
public record JugadorStatsDto(
    int Partidos,
    int Ganados,
    int Empatados,
    int Perdidos,
    int Goles,
    int Mvp,
    double GolesPorPartido);

/// <summary>Perfil completo: datos del jugador + sus estadísticas calculadas.</summary>
public record JugadorPerfilDto(
    long Id,
    string Nombre,
    string? Gamertag,
    string? PosicionHabitual,
    string? FotoUrl,
    bool Activo,
    JugadorStatsDto Stats);

/// <summary>Alta o edición de jugador. <c>Activo</c> nulo = true (sigue en el club).</summary>
public record JugadorGuardarDto(
    string Nombre,
    string? Gamertag,
    string? PosicionHabitual,
    string? FotoUrl,
    bool? Activo);