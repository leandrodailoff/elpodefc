# api/ — Backend ASP.NET Core (C#)

API REST de El Pode FC. **.NET 9 + EF Core + PostgreSQL** (Npgsql).
Contrato completo: `api-endpoints.md` (en `.clinerules/elpodefc`).

## Requisitos
- .NET SDK 9
- PostgreSQL local — servicio `db` del `docker-compose.yml` (raíz del repo), puerto **5433**
  en el host (el 5432 nativo de Windows es de otro postgres).

## Correr local

```bash
# 1) La BD (desde la raíz del repo)
docker compose up -d db

# 2) La API (usa appsettings.Development.json)
cd api
dotnet run            # http://localhost:8081 · OpenAPI en /openapi/v1.json
```

## La clave del club

Las **lecturas (GET) son públicas**; las **escrituras (POST/PUT/DELETE)** piden el header
`X-Club-Key`. En producción la clave vive en la variable de entorno `CLUB_KEY`; en local
está en `appsettings.Development.json` (= `elpodefc-dev-local`, solo desarrollo).

```bash
curl -X POST http://localhost:8081/api/jugadores \
  -H "X-Club-Key: elpodefc-dev-local" -H "Content-Type: application/json" \
  -d '{"nombre":"Lean","posicionHabitual":"DEL"}'
```

## Estructura

```
api/
├── Auth/          ← RequiereClaveAttribute (valida X-Club-Key en las escrituras)
├── Controllers/   ← un controller por sección del contrato
├── Dtos/          ← request/response JSON (record por caso de uso)
├── Domain/        ← entidades EF Core
├── Data/          ← ElPodeFCContext (mapeo, índices, CHECKs, seed del club)
├── Json/          ← FechaUtcConverter (normaliza fechas a UTC al entrar)
├── Mapping/       ← entidad → DTO
├── Services/      ← RelojClub ("¿se juega hoy?"), AlmacenMedios (archivos en disco)
├── Migrations/    ← migraciones EF Core
└── data/medios/   ← archivos subidos (gitignored; en la VPS: /data/medios)
```

## Decisiones de implementación que conviene recordar

- **Los archivos no van a la BD**: se guardan en `data/medios` (volumen) y se sirven en
  `/medios/<archivo>` con `UseStaticFiles`. La BD solo guarda la ruta.
- **Sin `UseHttpsRedirection`**: el SSL lo termina el proxy nginx; hacia la API el tráfico
  es HTTP interno y redirigir rompería el proxy.
- **Fechas**: Npgsql solo acepta offset 0 en `timestamptz`, así que `FechaUtcConverter`
  convierte a UTC todo lo que entra (el frontend manda la hora local que cargó el admin).
- **Tipo de medio derivado**: lo decide el servidor por la extensión o el link
  (`.png` → `imagen`, link de YouTube → `youtube`); nunca lo elige el cliente.
- **Stats siempre calculadas**: el récord, la racha, los goleadores y los MVP salen de
  consultas SQL, no hay un solo número guardado.

## Migraciones

```bash
cd api
dotnet ef migrations add <Nombre>   # una por cambio de esquema
dotnet ef database update           # aplica a la BD
dotnet ef migrations has-pending-model-changes   # verifica que no falte ninguna
```

## Probar a mano

El archivo `ElPodeFC.Api.http` tiene el flujo completo listo para ejecutar (VS Code →
REST Client, o directamente copiando los `curl`).