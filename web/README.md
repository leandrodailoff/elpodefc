# web/ — Frontend Angular (SPA)

SPA del sitio de El Pode FC. **Angular 21** (standalone, signals, sin SSR) + SCSS.
El contrato de API que consume está en `.clinerules/elpodefc/api-endpoints.md`.

## Requisitos

- **Node 22+** y **npm 11.19+** ⚠️ (npm 11.5.x tiene un bug de `arborist` que rompe
  `npm install` en este proyecto).
- La API corriendo en `http://localhost:8081` (ver [`../api/README.md`](../api/README.md)).

## Correr local

```bash
npm install
npm start        # ng serve → http://localhost:4200  (el proxy manda /api a :8081)
```

## Scripts

| Comando | Qué hace |
| --- | --- |
| `npm start` | Servidor de desarrollo (con proxy a la API) |
| `npm run build` | Build de producción en `dist/elpodefc-web` |
| `npm test` | Tests unitarios (vitest) |

## Estructura

```
src/
├── app/
│   ├── core/          ← ClubKey (clave del club: signal + localStorage) + interceptor
│   ├── features/      ← una carpeta por pantalla, todas con carga diferida
│   │   home/ · partidos/ · jugadores/ · estadisticas/
│   │   medios/ · formaciones/ · admin/
│   └── app.ts · app.html · app.scss · app.config.ts · app.routes.ts
├── index.html
└── styles.scss        ← variables de color y base
```

Pendiente de crear: `shared/` (componentes reusables: la cancha SVG, tarjetas de partido).

## Rutas

`/` · `/partidos` · `/partidos/:id` · `/jugadores` · `/jugadores/:id` ·
`/estadisticas` · `/medios` · `/formaciones` · `/admin`

## Dos cosas a tener presentes

- **`proxy.conf.json`** manda `/api` a `localhost:8081`: sin CORS en desarrollo, y el mismo
  código sirve en producción detrás del proxy nginx (mismo origen).
- **La clave del club** se guarda en `localStorage` y viaja **solo en escrituras**
  (header `X-Club-Key` por el interceptor); las lecturas son públicas y no la llevan.
