import { Routes } from '@angular/router';

/**
 * Rutas del sitio (mapa de pantallas de `analisis-idea.md`).
 *
 * Todas cargan en diferido (`loadComponent`): la primera visita baja solo el Inicio.
 * El detalle de partido y el perfil toman el `:id` como input del componente
 * (gracias a `withComponentInputBinding()` en `app.config.ts`).
 */
export const routes: Routes = [
  {
    path: '',
    title: 'El Pode FC',
    loadComponent: () => import('./features/home/home').then((m) => m.Home),
  },
  {
    path: 'partidos',
    title: 'Partidos · El Pode FC',
    loadComponent: () => import('./features/partidos/partidos').then((m) => m.Partidos),
  },
  {
    path: 'partidos/:id',
    title: 'Partido · El Pode FC',
    loadComponent: () =>
      import('./features/partidos/partido-detalle').then((m) => m.PartidoDetalle),
  },
  {
    path: 'jugadores',
    title: 'Plantel · El Pode FC',
    loadComponent: () => import('./features/jugadores/jugadores').then((m) => m.Jugadores),
  },
  {
    path: 'jugadores/:id',
    title: 'Jugador · El Pode FC',
    loadComponent: () =>
      import('./features/jugadores/jugador-perfil').then((m) => m.JugadorPerfil),
  },
  {
    path: 'estadisticas',
    title: 'Estadísticas · El Pode FC',
    loadComponent: () =>
      import('./features/estadisticas/estadisticas').then((m) => m.Estadisticas),
  },
  {
    path: 'medios',
    title: 'Grabaciones · El Pode FC',
    loadComponent: () => import('./features/medios/medios').then((m) => m.Medios),
  },
  {
    path: 'formaciones',
    title: 'La canchita · El Pode FC',
    loadComponent: () =>
      import('./features/formaciones/formaciones').then((m) => m.Formaciones),
  },
  {
    path: 'admin',
    title: 'Carga · El Pode FC',
    loadComponent: () => import('./features/admin/admin').then((m) => m.Admin),
  },
  { path: '**', redirectTo: '' },
];
