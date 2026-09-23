import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { EsquemaFormacion, Formacion } from './api-modelos';

/** La canchita: esquemas de formación (11 titular). */
@Injectable({ providedIn: 'root' })
export class FormacionesApi {
  private readonly http = inject(HttpClient);

  listar(): Observable<Formacion[]> {
    return this.http.get<Formacion[]>('/api/formaciones');
  }

  obtener(id: number): Observable<Formacion> {
    return this.http.get<Formacion>(`/api/formaciones/${id}`);
  }

  crear(nombre: string, esquema: EsquemaFormacion): Observable<Formacion> {
    return this.http.post<Formacion>('/api/formaciones', { nombre, esquema });
  }

  editar(id: number, nombre: string, esquema: EsquemaFormacion): Observable<Formacion> {
    return this.http.put<Formacion>(`/api/formaciones/${id}`, { nombre, esquema });
  }

  borrar(id: number): Observable<void> {
    return this.http.delete<void>(`/api/formaciones/${id}`);
  }
}