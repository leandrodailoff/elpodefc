using System.Text.Json;

namespace ElPodeFC.Api.Dtos;

/// <summary>
/// La "canchita": el esquema viaja como JSON real (no como string escapado).
/// Forma: <c>{ "titulares": [ { "pos": "POR", "x": 50, "y": 0, "jugadorId": 3 } ] }</c>
/// </summary>
public record FormacionDto(
    long Id,
    string Nombre,
    JsonElement Esquema,
    DateTimeOffset CreadoEn);

/// <summary>Alta o edición de un esquema de formación.</summary>
public record FormacionGuardarDto(
    string Nombre,
    JsonElement Esquema);