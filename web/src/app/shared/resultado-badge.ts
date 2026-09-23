import { Component, computed, input } from '@angular/core';

import { Resultado } from '../core/api-modelos';

const ETIQUETAS: Record<Resultado, string> = {
  G: 'Ganado',
  E: 'Empatado',
  P: 'Perdido',
};

/** La G/E/P con su color, igual en todo el sitio. */
@Component({
  selector: 'app-resultado-badge',
  template: `<span class="badge" [class]="'badge-' + clase()" [title]="etiqueta()">{{ resultado() }}</span>`,
  styles: `
    .badge {
      display: inline-block;
      min-width: 1.6rem;
      padding: 0.1rem 0.4rem;
      border-radius: 6px;
      font-size: 0.8rem;
      font-weight: 700;
      text-align: center;
      line-height: 1.4;
    }

    .badge-g {
      background: rgba(53, 196, 106, 0.18);
      color: var(--acento);
    }

    .badge-e {
      background: rgba(255, 255, 255, 0.1);
      color: var(--texto-suave);
    }

    .badge-p {
      background: rgba(227, 93, 106, 0.16);
      color: var(--alerta);
    }
  `,
})
export class ResultadoBadge {
  readonly resultado = input.required<Resultado>();

  protected readonly clase = computed(() => this.resultado().toLowerCase());
  protected readonly etiqueta = computed(() => ETIQUETAS[this.resultado()] ?? '');
}