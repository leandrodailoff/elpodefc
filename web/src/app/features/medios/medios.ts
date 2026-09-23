import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { MediosApi } from '../../core/medios-api';
import { carga } from '../../core/carga';
import { youtubeId, youtubeMiniatura } from '../../shared/video';

/** La galería: todas las fotos y videos del club. */
@Component({
  selector: 'app-medios',
  imports: [RouterLink],
  templateUrl: './medios.html',
  styleUrl: './medios.scss',
})
export class Medios {
  private readonly api = inject(MediosApi);

  readonly medios = carga(() => this.api.listar(), 'No pudimos cargar las grabaciones.');

  protected readonly youtubeId = youtubeId;
  protected readonly youtubeMiniatura = youtubeMiniatura;
}
