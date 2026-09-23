import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';

import { JugadoresApi } from '../../core/jugadores-api';
import { carga } from '../../core/carga';

/** El plantel: los del presente, los históricos, o todos. */
@Component({
  selector: 'app-jugadores',
  imports: [RouterLink],
  templateUrl: './jugadores.html',
  styleUrl: './jugadores.scss',
})
export class Jugadores {
  private readonly api = inject(JugadoresApi);

  readonly filtro = signal<'activos' | 'historicos' | 'todos'>('activos');

  /**
   * Se pide un objeto (no un booleano suelto) porque `undefined` es un valor válido
   * para el filtro "todos" y no queremos que lo confunda con "no hay nada que pedir".
   */
  private readonly filtros = computed(() => ({
    activo: this.filtro() === 'todos' ? undefined : this.filtro() === 'activos',
  }));

  readonly jugadores = carga(
    () => this.filtros(),
    (filtros) => this.api.listar(filtros.activo),
    'No pudimos cargar el plantel.',
  );

  protected readonly opciones: { valor: 'activos' | 'historicos' | 'todos'; etiqueta: string }[] = [
    { valor: 'activos', etiqueta: 'Plantel actual' },
    { valor: 'historicos', etiqueta: 'Históricos' },
    { valor: 'todos', etiqueta: 'Todos' },
  ];
}
