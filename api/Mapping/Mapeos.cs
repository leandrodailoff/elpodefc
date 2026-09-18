using ElPodeFC.Api.Domain;
using ElPodeFC.Api.Dtos;
using ElPodeFC.Api.Services;

namespace ElPodeFC.Api.Mapping;

/// <summary>Entidad → DTO. Las stats se arman en cada controller (son consultas, no campos).</summary>
public static class Mapeos
{
    public static JugadorDto ADto(this Jugador j) =>
        new(j.Id, j.Nombre, j.Gamertag, j.PosicionHabitual, j.FotoUrl, j.Activo);

    public static PartidoDto ADto(this Partido p, int cantidadMedios = 0) =>
        new(p.Id, p.Fecha, p.Rival, p.Resultado, p.MarcadorPropio, p.MarcadorRival,
            p.Tipo, p.Notas, p.RegistradoPor, cantidadMedios);

    public static ActuacionDto ADto(this Actuacion a) =>
        new(a.Id, a.JugadorId, a.Jugador?.Nombre ?? "", a.Posicion, a.Goles, a.Mvp);

    /// <summary>
    /// La URL que ve el frontend: el link externo si lo hay, o el archivo servido
    /// por la API bajo <c>/medios/</c>.
    /// </summary>
    public static MedioDto ADto(this Medio m) =>
        new(m.Id, m.Tipo,
            m.Url ?? (m.ArchivoRuta is null ? null : IAlmacenMedios.Url(m.ArchivoRuta)),
            m.ArchivoRuta, m.PartidoId, m.Titulo, m.SubidoPor, m.CreadoEn);

    public static FormacionDto ADto(this Formacion f) =>
        new(f.Id, f.Nombre, AJson(f.Esquema), f.CreadoEn);

    /// <summary>El esquema es texto en la BD pero viaja como JSON real (sin escapar).</summary>
    public static System.Text.Json.JsonElement AJson(string json)
    {
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch (System.Text.Json.JsonException)
        {
            return System.Text.Json.JsonDocument.Parse("{}").RootElement.Clone();
        }
    }
}