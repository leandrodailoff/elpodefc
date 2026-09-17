namespace ElPodeFC.Api.Domain;

/// <summary>Partido registrado del club. <see cref="Resultado"/> = 'G' | 'E' | 'P'.</summary>
public class Partido
{
    public long Id { get; set; }
    public DateTimeOffset Fecha { get; set; }
    public string? Rival { get; set; }
    public string Resultado { get; set; } = "E";
    public short MarcadorPropio { get; set; }
    public short MarcadorRival { get; set; }
    public string? Tipo { get; set; }
    public string? Notas { get; set; }
    public string? RegistradoPor { get; set; }
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ActualizadoEn { get; set; } = DateTimeOffset.UtcNow;

    public List<Actuacion> Actuaciones { get; set; } = [];
    public List<Medio> Medios { get; set; } = [];
}