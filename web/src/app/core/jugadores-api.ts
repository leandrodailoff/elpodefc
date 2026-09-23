import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Jugador, JugadorPerfil } from './api-modelos';

/** Datos que se mandan al dar de alta o editar un jugador. */
export interface JugadorGuardar {
  nombre: string;
  gamertag?: string | null;
  posicionHabitual?: string | null;
  fotoUrl?: string | null;
  activo?: boolean;
}

/** El plantel del club. */
@Injectable({ providedIn: 'root' })
export class JugadoresApi {
  private readonly http = inject(HttpClient);

  /** Lista el plantel. `activo` filtra presente (true) o históricos (false). */
  listar(activo?: boolean): Observable<Jugador[]> {
    let params = new HttpParams();
    if (activo !== undefined) params = params.set('activo', activo);

    return this.http.get<Jugador[]>('/api/jugadores', { params });
  }

  /** Perfil con las estadísticas calculadas por el backend. */
  obtener(id: number): Observable<JugadorPerfil> {
    return this.http.get<JugadorPerfil>(`/api/jugadores/${id}`);
  }

  crear(jugador: JugadorGuardar): Observable<Jugador> {
    return this.http.post<Jugador>('/api/jugadores', jugador);
  }

  editar(id: number, jugador: JugadorGuardar): Observable<Jugador> {
    return this.http.put<Jugador>(`/api/jugadores/${id}`, jugador);
  }

  borrar(id: number): Observable<void> {
    return this.http.delete<void>(`/api/jugadores/${id}`);
  }
}