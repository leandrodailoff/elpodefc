import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

import { PartidosApi } from '../../core/partidos-api';
import { FiltrosPartidos } from '../../core/partidos-api';
import { carga } from '../../core/carga';
import { Resultado } from '../../core/api-modelos';
import { ResultadoBadge } from '../../shared/resultado-badge';

/** El historial completo, con filtro por resultado. */
@Component({
  selector: 'app-partidos',
  imports: [RouterLink, DatePipe, ResultadoBadge],
  templateUrl: './partidos.html',
  styleUrl: './partidos.scss',
})
export class Partidos {
  private readonly api = inject(PartidosApi);

  /** Filtro activo (vacío = todos). Es un signal: cambiar el chip recarga la lista. */
  readonly resultado = signal<Resultado | ''>('');

  private readonly filtros = computed<FiltrosPartidos>(() => ({ resultado: this.resultado() }));

  readonly partidos = carga(
    () => this.filtros(),
    (filtros) => this.api.listar(filtros),
    'No pudimos cargar el historial de partidos.',
  );

  protected readonly opciones: { valor: Resultado | ''; etiqueta: string }[] = [
    { valor: '', etiqueta: 'Todos' },
    { valor: 'G', etiqueta: 'Ganados' },
    { valor: 'E', etiqueta: 'Empatados' },
    { valor: 'P', etiqueta: 'Perdidos' },
  ];
}
