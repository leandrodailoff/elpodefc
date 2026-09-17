namespace ElPodeFC.Api.Domain;

/// <summary>Ficha del club: horario habitual, días de juego y Discord. Fila única (Id = 1).</summary>
public class ClubConfig
{
    public short Id { get; set; } = 1;
    public string? DiscordUrl { get; set; }
    public TimeOnly? HorarioJuego { get; set; }
    public string[] DiasJuego { get; set; } = [];
    public bool Pausado { get; set; }
    public DateTimeOffset ActualizadoEn { get; set; } = DateTimeOffset.UtcNow;
}