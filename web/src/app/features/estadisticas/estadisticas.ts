import { Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';

import { ClubApi } from '../../core/club-api';
import { carga } from '../../core/carga';
import { ResultadoBadge } from '../../shared/resultado-badge';

/** El panel del club: récord, racha, goleadores y MVP. Todo lo calcula el backend. */
@Component({
  selector: 'app-estadisticas',
  imports: [RouterLink, ResultadoBadge],
  templateUrl: './estadisticas.html',
  styleUrl: './estadisticas.scss',
})
export class Estadisticas {
  private readonly api = inject(ClubApi);

  readonly stats = carga(() => this.api.estadisticas(), 'No pudimos cargar las estadísticas.');

  /** "3 victorias al hilo" (o su versión en singular). */
  protected readonly textoRacha = computed(() => {
    const racha = this.stats.dato()?.racha;
    if (!racha || !racha.tipo) return 'Todavía no hay partidos';

    const palabra = racha.tipo === 'G' ? 'victoria' : racha.tipo === 'E' ? 'empate' : 'derrota';
    return `${racha.cantidad} ${palabra}${racha.cantidad === 1 ? '' : 's'} al hilo`;
  });

  /** Diferencia de gol: en un club de amigos el signo importa más que el número. */
  protected readonly diferencia = computed(() => {
    const s = this.stats.dato();
    if (!s) return 0;
    return s.golesPropios - s.golesRivales;
  });
}
