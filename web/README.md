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
npm start        # ng serve → http://localhost:4200  (el proxy manda /api y /medios a :8081)
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
│   ├── core/          ← lo que no es pantalla
│   │   ├── api-modelos.ts        tipos del contrato (los nombres siguen el JSON)
│   │   ├── carga.ts              Observable → señales de dato / cargando / error (+ recargar)
│   │   ├── club-api.ts           ficha del club y estadísticas
│   │   ├── jugadores-api.ts      plantel y perfiles
│   │   ├── partidos-api.ts       historial, detalle y ABM
│   │   ├── medios-api.ts         galería y subida (FormData)
│   │   ├── formaciones-api.ts    la canchita
│   │   ├── club-key.ts           la clave del club (signal + localStorage)
│   │   └── club-key-interceptor.ts   manda X-Club-Key solo en las escrituras
│   ├── shared/        ← piezas reusables
│   │   ├── resultado-badge.ts    la G/E/P con su color
│   │   ├── horario.ts            abreviaturas de días → texto ("vie" → "Viernes")
│   │   └── video.ts              links de YouTube: id, miniatura y embed
│   ├── features/      ← una carpeta por pantalla, todas con carga diferida
│   │   home/ · partidos/ · jugadores/ · estadisticas/
│   │   medios/ · formaciones/ · admin/
│   └── app.ts · app.html · app.scss · app.config.ts · app.routes.ts
├── index.html
└── styles.scss        ← variables de color + piezas reusables (tarjeta, grilla, tabla, boton)
```

## Rutas

`/` · `/partidos` · `/partidos/:id` · `/jugadores` · `/jugadores/:id` ·
`/estadisticas` · `/medios` · `/formaciones` · `/admin`

## Cómo se pide un dato (el patrón de todas las pantallas)

```ts
readonly club = carga(() => this.clubApi.obtener(), 'No pudimos cargar los datos del club.');
```

`carga()` (en `core/carga.ts`) devuelve `dato()`, `cargando()`, `error()` y `recargar()`, y
corta la suscripción sola si la pantalla se destruye. Con parámetro reactivo, se recarga
cuando cambia:

```ts
readonly partido = carga(() => Number(this.id()), (id) => this.partidosApi.obtener(id));
```

En el template se usan las tres señales y un botón de reintentar:

```html
@if (club.cargando()) { … } @else if (club.error()) { … } @else { {{ club.dato()!.nombre }} }
```

## Dos cosas a tener presentes

- **`proxy.conf.json`** manda `/api` y `/medios` a `localhost:8081`: sin CORS en desarrollo,
  y el mismo código sirve en producción detrás del proxy nginx (mismo origen).
- **La clave del club** se guarda en `localStorage` y viaja **solo en escrituras**
  (header `X-Club-Key` por el interceptor); las lecturas son públicas y no la llevan.

## Tests

`npm test` corre los specs con `HttpTestingController` (sin tocar la API real): verifican
que cada pantalla pida lo que corresponde, pinte los datos, muestre el error y recargue
cuando cambia un filtro.
