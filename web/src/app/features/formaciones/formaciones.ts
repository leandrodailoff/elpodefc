import { Component, computed, inject, signal } from '@angular/core';

import { FormacionesApi } from '../../core/formaciones-api';
import { JugadoresApi } from '../../core/jugadores-api';
import { carga } from '../../core/carga';
import { Formacion } from '../../core/api-modelos';

/**
 * La canchita: el 11 titular dibujado sobre el campo.
 *
 * El esquema viene del backend como JSON libre (posiciones y jugadores), así que mover
 * un jugador de puesto es cambiar datos, no código. Los nombres se resuelven contra el
 * plantel, porque el esquema solo guarda el `jugadorId`.
 */
@Component({
  selector: 'app-formaciones',
  imports: [],
  templateUrl: './formaciones.html',
  styleUrl: './formaciones.scss',
})
export class Formaciones {
  private readonly api = inject(FormacionesApi);
  private readonly jugadoresApi = inject(JugadoresApi);

  readonly formaciones = carga(() => this.api.listar(), 'No pudimos cargar las formaciones.');
  readonly jugadores = carga(() => this.jugadoresApi.listar(), 'No pudimos cargar el plantel.');

  /** Cuál se está mirando. Null = la primera de la lista. */
  readonly seleccionada = signal<number | null>(null);

  readonly actual = computed<Formacion | null>(() => {
    const lista = this.formaciones.dato();
    if (!lista?.length) return null;

    const id = this.seleccionada();
    return lista.find((f) => f.id === id) ?? lista[0];
  });

  /** El nombre del jugador puesto en la cancha (el esquema solo guarda el id). */
  protected nombre(jugadorId: number): string {
    return this.jugadores.dato()?.find((j) => j.id === jugadorId)?.nombre ?? `#${jugadorId}`;
  }
}
