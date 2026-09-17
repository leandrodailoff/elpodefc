namespace ElPodeFC.Api.Domain;

/// <summary>Jugador del club. <see cref="Activo"/> = true también es "histórico del club".</summary>
public class Jugador
{
    public long Id { get; set; }
    public string Nombre { get; set; } = "";
    public string? Gamertag { get; set; }
    public string? PosicionHabitual { get; set; }
    public string? FotoUrl { get; set; }
    public bool Activo { get; set; } = true;
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ActualizadoEn { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Actuacion> Actuaciones { get; set; } = [];
}