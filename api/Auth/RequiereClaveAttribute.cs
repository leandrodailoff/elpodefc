using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ElPodeFC.Api.Auth;

/// <summary>
/// Exige el header <c>X-Club-Key</c> en las escrituras (POST/PUT/DELETE).
///
/// La clave vive en la variable de entorno <c>CLUB_KEY</c> (nunca hardcodeada, ver
/// <c>api-endpoints.md</c>). Las lecturas (GET) NO llevan este atributo: son públicas.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequiereClaveAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>Nombre del header que manda el frontend en las escrituras.</summary>
    public const string Header = "X-Club-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var esperada = context.HttpContext.RequestServices
            .GetRequiredService<IConfiguration>()["CLUB_KEY"];

        if (string.IsNullOrWhiteSpace(esperada))
        {
            context.Result = new ObjectResult(new { error = "CLUB_KEY no está configurada en el servidor." })
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
            return;
        }

        var recibida = context.HttpContext.Request.Headers[Header].ToString();

        if (!string.Equals(recibida, esperada, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Clave del club inválida o ausente." });
            return;
        }

        await next();
    }
}