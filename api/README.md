# api/ — Backend ASP.NET Core (C#)

API REST de El Pode FC. **.NET 9 + EF Core + PostgreSQL** (Npgsql).

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
dotnet run
```

## Migraciones

```bash
cd api
dotnet ef migrations add <Nombre>   # nueva migración (una por cambio de esquema)
dotnet ef database update           # aplica a la BD
```

Referencia del contrato: `api-endpoints.md` (en `.clinerules/elpodefc`).