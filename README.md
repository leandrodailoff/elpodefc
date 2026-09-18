# elpodefc

Web del club Pro **El Pode FC** (EA Sports FC): partidos, estadísticas, medios y la canchita.

**Stack:** Angular (SPA) · ASP.NET Core (C#) — API REST · PostgreSQL · Docker

## Estado

- Esqueleto inicial: estructura de monorepo + PostgreSQL local en Docker.
- Backend `api/`: **ASP.NET Core 9** con las entidades del esquema v1, EF Core +
  Npgsql y la **migración `InitialCreate` aplicada** a la BD local (puerto 5433).
  **API v1 completa**: jugadores, partidos (alta en una llamada), medios, club +
  estadísticas y formaciones, con clave compartida (`X-Club-Key`) en las escrituras.
- Frontend `web/`: **Angular 21** (standalone, signals, sin SSR) con las 9 pantallas
  del mapa de secciones, layout propio, proxy `/api` y la clave del club en `core/`.
- La documentación viva de la idea (análisis, BD, API, estructura) está en
  `d:\Proyectos\.clinerules\elpodefc` y se refleja en [`docs/`](docs/).

## Requisitos

- Docker Desktop corriendo en la máquina (o Docker Engine en Linux).
- **.NET SDK 9** para la API y **Node 22+ / npm 11.19+** para el frontend.

## Arranque local

```bash
docker compose up -d db        # 1) base de datos (esperar "healthy")
cd api && dotnet run           # 2) API en http://localhost:8081
cd web && npm install && npm start   # 3) SPA en http://localhost:4200
```

Parar: `docker compose stop db` · Borrar todo (datos incluidos): `docker compose down -v`

## Próximos pasos

- [`api/`](api/) — ✅ API v1 completa (ver `api/README.md` y `ElPodeFC.Api.http`).
- [`web/`](web/) — consumir la API en cada sección (pendiente).
- [`deploy/`](deploy/) — compose completo + proxy + SSL · layout en `estructura-proyecto.md`
  (en `.clinerules/elpodefc`).