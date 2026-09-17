using ElPodeFC.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// PostgreSQL (EF Core). Cadena de conexión:
//  - dev:    appsettings.Development.json → "Default"
//  - prod:   variable de entorno "ConnectionStrings__Default" (la setea el .env de la VPS)
builder.Services.AddDbContext<ElPodeFCContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
