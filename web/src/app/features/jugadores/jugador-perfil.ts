import { Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';

import { JugadoresApi } from '../../core/jugadores-api';
import { carga } from '../../core/carga';

/**
 * El perfil de un jugador con sus estadísticas individuales.
 *
 * Los números NO se calculan acá: los deriva el backend de sus actuaciones.
 */
@Component({
  selector: 'app-jugador-perfil',
  imports: [RouterLink],
  templateUrl: './jugador-perfil.html',
  styleUrl: './jugador-perfil.scss',
})
export class JugadorPerfil {
  private readonly api = inject(JugadoresApi);

  /** Id del jugador, bindeado desde la ruta `/jugadores/:id`. */
  readonly id = input.required<string>();

  readonly jugador = carga(
    () => Number(this.id()),
    (id) => this.api.obtener(id),
    'No pudimos cargar el jugador.',
  );
}

