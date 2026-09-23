import { Component, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

import { PartidosApi } from '../../core/partidos-api';
import { carga } from '../../core/carga';
import { ResultadoBadge } from '../../shared/resultado-badge';
import { youtubeEmbed, youtubeId, youtubeMiniatura } from '../../shared/video';

/**
 * El detalle de un partido: cómo salió, quién jugó y las grabaciones.
 *
 * El id llega como input desde la ruta (`/partidos/:id`), así que la pantalla se
 * recarga sola si cambia.
 */
@Component({
  selector: 'app-partido-detalle',
  imports: [RouterLink, DatePipe, ResultadoBadge],
  templateUrl: './partido-detalle.html',
  styleUrl: './partido-detalle.scss',
})
export class PartidoDetalle {
  private readonly api = inject(PartidosApi);

  /** Id del partido, bindeado desde la ruta `/partidos/:id`. */
  readonly id = input.required<string>();

  readonly partido = carga(
    () => Number(this.id()),
    (id) => this.api.obtener(id),
    'No pudimos cargar el partido.',
  );

  /** Id del medio que se está reproduciendo en la página (los videos se abren al tocarlos). */
  protected readonly reproduciendo = signal<number | null>(null);

  // Ayudas de video, para no repetir la lógica en el template.
  protected readonly youtubeId = youtubeId;
  protected readonly youtubeMiniatura = youtubeMiniatura;
  protected readonly youtubeEmbed = youtubeEmbed;
}

