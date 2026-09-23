import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Partido, PartidoDetalle, Resultado } from './api-modelos';

/** Filtros del historial (todos opcionales). */
export interface FiltrosPartidos {
  resultado?: Resultado | '';
  tipo?: string;
  desde?: string;
  hasta?: string;
  limit?: number;
}

/** Una actuación en el payload de alta/edición. */
export interface ActuacionGuardar {
  jugadorId: number;
  posicion?: string | null;
  goles: number;
  mvp: boolean;
}

/**
 * Partido que se manda al guardar. `resultado` es opcional: si no va, el servidor
 * lo deriva de los marcadores.
 */
export interface PartidoGuardar {
  fecha: string;
  rival?: string | null;
  resultado?: Resultado | null;
  marcadorPropio: number;
  marcadorRival: number;
  tipo?: string | null;
  notas?: string | null;
  registradoPor?: string | null;
  actuaciones: ActuacionGuardar[];
}

/** Historial y detalle de partidos. */
@Injectable({ providedIn: 'root' })
export class PartidosApi {
  private readonly http = inject(HttpClient);

  listar(filtros: FiltrosPartidos = {}): Observable<Partido[]> {
    let params = new HttpParams();

    if (filtros.resultado) params = params.set('resultado', filtros.resultado);
    if (filtros.tipo) params = params.set('tipo', filtros.tipo);
    if (filtros.desde) params = params.set('desde', filtros.desde);
    if (filtros.hasta) params = params.set('hasta', filtros.hasta);
    if (filtros.limit) params = params.set('limit', filtros.limit);

    return this.http.get<Partido[]>('/api/partidos', { params });
  }

  /** Detalle completo: actuaciones + grabaciones del partido. */
  obtener(id: number): Observable<PartidoDetalle> {
    return this.http.get<PartidoDetalle>(`/api/partidos/${id}`);
  }

  /** Un solo POST crea el partido completo (datos + actuaciones). */
  crear(partido: PartidoGuardar): Observable<PartidoDetalle> {
    return this.http.post<PartidoDetalle>('/api/partidos', partido);
  }

  editar(id: number, partido: PartidoGuardar): Observable<PartidoDetalle> {
    return this.http.put<PartidoDetalle>(`/api/partidos/${id}`, partido);
  }

  borrar(id: number): Observable<void> {
    return this.http.delete<void>(`/api/partidos/${id}`);
  }
}