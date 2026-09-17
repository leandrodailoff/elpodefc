namespace ElPodeFC.Api.Domain;

/// <summary>El "puente": qué hizo un jugador en un partido. De acá salen TODAS las stats.</summary>
public class Actuacion
{
    public long Id { get; set; }
    public long PartidoId { get; set; }
    public long JugadorId { get; set; }
    public string? Posicion { get; set; }
    public short Goles { get; set; }
    public bool Mvp { get; set; }

    public Partido Partido { get; set; } = null!;
    public Jugador Jugador { get; set; } = null!;
}