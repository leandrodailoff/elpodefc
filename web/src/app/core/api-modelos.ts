/**
 * Tipos del contrato de la API (ver `api-endpoints.md` en `.clinerules/elpodefc`).
 *
 * Los nombres siguen el JSON que devuelve el backend (camelCase). Las fechas son
 * **UTC en ISO 8601**: al mostrarlas se convierten a la hora local del navegador.
 */

/** Ficha del club + el "¿se juega hoy?" que calcula el servidor. */
export interface Club {
  discordUrl: string | null;
  horarioJuego: string | null;
  diasJuego: string[];
  pausado: boolean;
  juegaHoy: boolean;
  diaDeHoy: string;
}

export interface Jugador {
  id: number;
  nombre: string;
  gamertag: string | null;
  posicionHabitual: string | null;
  fotoUrl: string | null;
  activo: boolean;
}

/** Stats derivadas: nunca se guardan, las calcula el backend. */
export interface JugadorStats {
  partidos: number;
  ganados: number;
  empatados: number;
  perdidos: number;
  goles: number;
  mvp: number;
  golesPorPartido: number;
}

export interface JugadorPerfil extends Jugador {
  stats: JugadorStats;
}

/** 'G' ganado · 'E' empatado · 'P' perdido. */
export type Resultado = 'G' | 'E' | 'P';

export interface Partido {
  id: number;
  fecha: string;
  rival: string | null;
  resultado: Resultado;
  marcadorPropio: number;
  marcadorRival: number;
  tipo: string | null;
  notas: string | null;
  registradoPor: string | null;
  cantidadMedios: number;
}

export interface Actuacion {
  id: number;
  jugadorId: number;
  jugador: string;
  posicion: string | null;
  goles: number;
  mvp: boolean;
}

/** 'imagen' | 'video' | 'youtube' (lo deriva el servidor). */
export type TipoMedio = 'imagen' | 'video' | 'youtube';

export interface Medio {
  id: number;
  tipo: TipoMedio;
  url: string | null;
  archivoRuta: string | null;
  partidoId: number | null;
  titulo: string | null;
  subidoPor: string | null;
  creadoEn: string;
}

export interface PartidoDetalle {
  id: number;
  fecha: string;
  rival: string | null;
  resultado: Resultado;
  marcadorPropio: number;
  marcadorRival: number;
  tipo: string | null;
  notas: string | null;
  registradoPor: string | null;
  actuaciones: Actuacion[];
  medios: Medio[];
}

export interface Ranking {
  jugadorId: number;
  jugador: string;
  cantidad: number;
}

export interface Racha {
  tipo: Resultado | '';
  cantidad: number;
}

export interface Estadisticas {
  partidos: number;
  ganados: number;
  empatados: number;
  perdidos: number;
  golesPropios: number;
  golesRivales: number;
  racha: Racha;
  goleadores: Ranking[];
  mvps: Ranking[];
  ultimosResultados: Resultado[];
}

/** Un jugador puesto en la cancha (el `esquema` es libre: es JSONB). */
export interface Titular {
  pos: string;
  x: number;
  y: number;
  jugadorId: number;
}

export interface EsquemaFormacion {
  formacion?: string;
  titulares: Titular[];
}

export interface Formacion {
  id: number;
  nombre: string;
  esquema: EsquemaFormacion;
  creadoEn: string;
}