namespace ElPodeFC.Api.Domain;

/// <summary>La "canchita": esquema del 11 titular. <see cref="Esquema"/> es JSONB.</summary>
public class Formacion
{
    public long Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Esquema { get; set; } = "{}";   // { "titulares": [ { "pos", "x", "y", "jugadorId" }, ... ] }
    public DateTimeOffset CreadoEn { get; set; } = DateTimeOffset.UtcNow;
}