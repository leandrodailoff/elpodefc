import { DestroyRef, Signal, effect, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable } from 'rxjs';

/**
 * El resultado de una carga: el dato, si está cargando, el error y cómo reintentar.
 *
 * Las tres señales son de solo lectura para el componente: la carga las maneja esta pieza.
 */
export interface Carga<T> {
  readonly dato: Signal<T | null>;
  readonly cargando: Signal<boolean>;
  readonly error: Signal<string | null>;

  /** Vuelve a pedir el dato (para el botón "reintentar" o después de guardar algo). */
  recargar(): void;
}

/** Se dispara cuando la fuente falla: ni el componente ni el usuario necesitan el detalle técnico. */
const MENSAJE_POR_DEFECTO = 'No pudimos cargar los datos. Probá de nuevo en un rato.';

/** Valores con los que NO hay que pedir nada (por ejemplo, un `:id` que todavía no llegó). */
function falta(valor: unknown): boolean {
  return (
    valor === undefined ||
    valor === null ||
    valor === '' ||
    (typeof valor === 'number' && Number.isNaN(valor))
  );
}

/**
 * Envuelve un pedido HTTP en señales de carga/error, sin boilerplate en cada pantalla.
 *
 * ```ts
 * // sin parámetros
 * readonly club = carga(() => this.clubApi.obtener());
 *
 * // con parámetro reactivo (se recarga solo cuando cambia el id de la ruta)
 * readonly partido = carga(
 *   () => Number(this.id()),
 *   (id) => this.partidosApi.obtener(id),
 *   'No pudimos cargar el partido.',
 * );
 * ```
 *
 * Se apoya en `takeUntilDestroyed`: si la pantalla se destruye mientras el pedido viaja,
 * la suscripción se corta sola (no hay fugas ni errores por respuestas tardías).
 */
export function carga<T>(fuente: () => Observable<T>, mensajeError?: string): Carga<T>;
export function carga<P, T>(
  parametros: () => P,
  fuente: (parametros: P) => Observable<T>,
  mensajeError?: string,
): Carga<T>;
export function carga(
  primero: () => unknown,
  segundo?: unknown,
  tercero?: string,
): Carga<unknown> {
  const destruir = inject(DestroyRef);

  const conParametros = typeof segundo === 'function';
  const leerParametros = conParametros ? (primero as () => unknown) : null;
  const fuente = (conParametros ? segundo : primero) as (p: unknown) => Observable<unknown>;
  const mensaje = (conParametros ? tercero : (segundo as string | undefined)) ?? MENSAJE_POR_DEFECTO;

  const _dato = signal<unknown>(null);
  const _cargando = signal(true);
  const _error = signal<string | null>(null);

  const ejecutar = (parametros: unknown): void => {
    _cargando.set(true);
    _error.set(null);

    fuente(parametros)
      .pipe(takeUntilDestroyed(destruir))
      .subscribe({
        next: (valor) => {
          _dato.set(valor);
          _cargando.set(false);
        },
        error: (falla: unknown) => {
          console.error('Falló la carga de datos:', falla);
          _error.set(mensaje);
          _cargando.set(false);
        },
      });
  };

  if (leerParametros) {
    // El efecto vuelve a pedir el dato cuando cambia el parámetro (y la primera vez).
    effect(() => {
      const parametros = leerParametros();
      if (falta(parametros)) return;

      ejecutar(parametros);
    });
  } else {
    ejecutar(undefined);
  }

  return {
    dato: _dato.asReadonly(),
    cargando: _cargando.asReadonly(),
    error: _error.asReadonly(),
    recargar: () => ejecutar(leerParametros ? leerParametros() : undefined),
  };
}