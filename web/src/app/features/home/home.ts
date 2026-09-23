import { Component, inject } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

import { ClubApi } from '../../core/club-api';
import { PartidosApi } from '../../core/partidos-api';
import { carga } from '../../core/carga';
import { ResultadoBadge } from '../../shared/resultado-badge';
import { textoDias, textoHora } from '../../shared/horario';

/**
 * El Inicio: el estado del club (¿se juega hoy?) y los últimos resultados.
 *
 * Son dos pedidos independientes: si uno falla, el otro se muestra igual.
 */
@Component({
  selector: 'app-home',
  imports: [RouterLink, DatePipe, ResultadoBadge],
  templateUrl: './home.html',
  styleUrl: './home.scss',
})
export class Home {
  private readonly clubApi = inject(ClubApi);
  private readonly partidosApi = inject(PartidosApi);

  readonly club = carga(() => this.clubApi.obtener(), 'No pudimos cargar los datos del club.');
  readonly ultimos = carga(
    () => this.partidosApi.listar({ limit: 5 }),
    'No pudimos cargar los últimos partidos.',
  );

  // Se exponen al template para no repetir la lógica de formato.
  protected readonly textoDias = textoDias;
  protected readonly textoHora = textoHora;
}
