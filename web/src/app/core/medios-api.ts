import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { Medio } from './api-modelos';

/** Fotos y grabaciones. */
@Injectable({ providedIn: 'root' })
export class MediosApi {
  private readonly http = inject(HttpClient);

  /** Galería general, o las grabaciones de un partido con `partidoId`. */
  listar(partidoId?: number): Observable<Medio[]> {
    let params = new HttpParams();
    if (partidoId !== undefined) params = params.set('partido_id', partidoId);

    return this.http.get<Medio[]>('/api/medios', { params });
  }

  /**
   * Sube un medio. Va **siempre** como `FormData` (el endpoint es multipart):
   * con `archivo` (foto/video) o con `url` (por ejemplo, un link de YouTube).
   * El `tipo` lo deriva el servidor, no se manda.
   */
  subir(datos: { archivo?: File; url?: string; partidoId?: number; titulo?: string }): Observable<Medio> {
    const form = new FormData();

    if (datos.archivo) form.append('Archivo', datos.archivo);
    if (datos.url) form.append('Url', datos.url);
    if (datos.partidoId !== undefined) form.append('PartidoId', String(datos.partidoId));
    if (datos.titulo) form.append('Titulo', datos.titulo);

    return this.http.post<Medio>('/api/medios', form);
  }

  borrar(id: number): Observable<void> {
    return this.http.delete<void>(`/api/medios/${id}`);
  }
}