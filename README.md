# elpodefc

Web del club Pro **El Pode FC** (EA Sports FC): partidos, estadísticas, medios y la canchita.

**Stack:** Angular (SPA) · ASP.NET Core (C#) — API REST · PostgreSQL · Docker

## Estado

- Esqueleto inicial: estructura de monorepo + PostgreSQL local en Docker.
- La documentación viva de la idea (análisis, BD, API, estructura) está en
  `d:\Proyectos\.clinerules\elpodefc` y se refleja en [`docs/`](docs/).

## Requisitos

- Docker Desktop corriendo en la máquina (o Docker Engine en Linux).

## Arranque local (solo la BD por ahora)

```bash
docker compose up -d db
docker compose ps            # esperar a que esté healthy
docker exec -it elpodefc-db psql -U elpodefc -d elpodefc   # consola
```

Parar: `docker compose stop db` · Borrar todo (datos incluidos): `docker compose down -v`

## Próximos pasos

- [`api/`](api/) — ASP.NET Core · [`web/`](web/) — Angular · [`deploy/`](deploy/) —
  layout completo en `estructura-proyecto.md` (en `.clinerules/elpodefc`).