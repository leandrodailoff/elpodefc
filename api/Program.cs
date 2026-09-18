using ElPodeFC.Api.Data;
using ElPodeFC.Api.Json;
using ElPodeFC.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // La base guarda instantes en UTC; el frontend manda la hora local que se cargó.
        o.JsonSerializerOptions.Converters.Add(new FechaUtcConverter());
    });

builder.Services.AddOpenApi();

// PostgreSQL (EF Core). Cadena de conexión:
//  - dev:    appsettings.Development.json → "Default"
//  - prod:   variable de entorno "ConnectionStrings__Default" (la setea el .env de la VPS)
builder.Services.AddDbContext<ElPodeFCContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// La clave del club (escrituras) y el reloj que decide "¿se juega hoy?" con la zona del club.
builder.Services.AddSingleton<RelojClub>();
builder.Services.AddSingleton<IAlmacenMedios, AlmacenMedios>();

// Los videos de 500 MB no entran en el límite por defecto de Kestrel (30 MB).
builder.WebHost.ConfigureKestrel(o => o.Limits.MaxRequestBodySize = 500L * 1024 * 1024);

// En desarrollo el frontend corre en otro puerto (Angular :4200). En producción van por
// el mismo dominio vía proxy nginx → no hay CORS que configurar.
const string AngularDev = "angular-dev";
builder.Services.AddCors(o => o.AddPolicy(AngularDev, p => p
    .WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors(AngularDev);
}

// OJO: no se usa UseHttpsRedirection. El SSL lo termina el proxy nginx y hacia la API
// el tráfico es HTTP interno; redirigir acá rompería el proxy (bucle de redirects).

// Los archivos de medios se sirven desde el volumen (en la VPS: /data/medios),
// nunca desde la base: la BD solo guarda la ruta.
var raizConfigurada = app.Configuration["Almacen:Raiz"] ?? "data/medios";
var raizMedios = Path.IsPathRooted(raizConfigurada)
    ? raizConfigurada
    : Path.Combine(app.Environment.ContentRootPath, raizConfigurada);

Directory.CreateDirectory(raizMedios);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(raizMedios),
    RequestPath = "/medios",
});

app.UseAuthorization();
app.MapControllers();

app.Run();
