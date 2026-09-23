import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Club, Estadisticas } from './api-modelos';

/** Ficha del club y el panel de estadísticas. */
@Injectable({ providedIn: 'root' })
export class ClubApi {
  private readonly http = inject(HttpClient);

  /** Datos del club: `juegaHoy` ya viene calculado por el servidor. */
  obtener(): Observable<Club> {
    return this.http.get<Club>('/api/club');
  }

  /** Edita la ficha (requiere la clave: la agrega el interceptor). */
  editar(club: Omit<Club, 'juegaHoy' | 'diaDeHoy'>): Observable<Club> {
    return this.http.put<Club>('/api/club', club);
  }

  /** Panel del club: récord, racha, goleadores y MVP (todo derivado). */
  estadisticas(): Observable<Estadisticas> {
    return this.http.get<Estadisticas>('/api/club/estadisticas');
  }
}