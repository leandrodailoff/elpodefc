using System.Globalization;

namespace ElPodeFC.Api.Services;

/// <summary>
/// Resuelve el "hoy" del club con SU zona horaria (nunca la del teléfono que consulta:
/// un jugador de vacaciones no puede hacer que la página diga "hoy no se juega").
/// </summary>
public sealed class RelojClub
{
    /// <summary>Abreviaturas de día usadas en <c>club_config.dias_juego</c> y en toda la API.</summary>
    public static readonly string[] Dias = ["dom", "lun", "mar", "mie", "jue", "vie", "sab"];

    private readonly TimeZoneInfo _zona;

    public RelojClub(IConfiguration config)
    {
        _zona = Resolver(config["Club:ZonaHoraria"]);
    }

    /// <summary>La hora actual del club (para mostrar el horario sin sustos de huso).</summary>
    public DateTimeOffset AhoraLocal => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _zona);

    /// <summary>El día de hoy en el club, en la abreviatura que guarda la BD (ej: "mar").</summary>
    public string DiaDeHoy => Dias[(int)AhoraLocal.DayOfWeek];

    /// <summary>¿Hay partido hoy? = el club no está pausado Y hoy es día de juego.</summary>
    public bool JuegaHoy(IEnumerable<string> diasJuego, bool pausado) =>
        !pausado && diasJuego.Contains(DiaDeHoy);

    private static TimeZoneInfo Resolver(string? id)
    {
        if (string.IsNullOrWhiteSpace(id)) return TimeZoneInfo.Local;

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            // La config no existe en este SO: mejor la hora local que romper el sitio.
            return TimeZoneInfo.Local;
        }
    }

    /// <summary>Normaliza un día que llega del frontend ("MAR", "Mar" → "mar") o null si es inválido.</summary>
    public static string? NormalizarDia(string? dia)
    {
        if (string.IsNullOrWhiteSpace(dia)) return null;

        var limpio = dia.Trim().ToLowerInvariant();
        limpio = limpio.Replace("é", "e").Replace("á", "a").Replace("ó", "o").Replace("í", "i").Replace("ú", "u");

        return Dias.Contains(limpio) ? limpio : null;
    }
}