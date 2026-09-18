import { Component, input } from '@angular/core';

/** Detalle de un partido: actuaciones + grabaciones (contrato: `GET /api/partidos/{id}`). */
@Component({
  selector: 'app-partido-detalle',
  imports: [],
  templateUrl: './partido-detalle.html',
  styleUrl: './partido-detalle.scss',
})
export class PartidoDetalle {
  /** Id del partido, bindeado desde la ruta `/partidos/:id`. */
  readonly id = input.required<string>();
}

