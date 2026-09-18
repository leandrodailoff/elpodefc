using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElPodeFC.Api.Json;

/// <summary>
/// Normaliza a UTC toda fecha que entra por JSON.
///
/// PostgreSQL (<c>timestamptz</c>) solo acepta offset 0, y el frontend manda la hora
/// local que cargó el admin (ej: <c>2026-09-17T22:00:00-03:00</c>). Convertir en el
/// borde evita el error "Cannot write DateTimeOffset with Offset=-03:00:00" y deja
/// la base siempre en UTC, que es lo correcto para guardar instantes.
/// </summary>
public sealed class FechaUtcConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader, Type tipo, JsonSerializerOptions opciones) =>
        reader.GetDateTimeOffset().ToUniversalTime();

    public override void Write(
        Utf8JsonWriter writer, DateTimeOffset valor, JsonSerializerOptions opciones) =>
        writer.WriteStringValue(valor.ToUniversalTime());
}