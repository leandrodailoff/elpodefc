import { Injectable, computed, signal } from '@angular/core';

/** Dónde se recuerda la clave entre visitas (solo en el navegador del que carga datos). */
const CLAVE_STORAGE = 'elpodefc.clubKey';

/**
 * La clave compartida del club (v1 sin usuarios: ver `api-endpoints.md`).
 *
 * Se guarda en el navegador de quien va a cargar datos y el interceptor la manda
 * como header `X-Club-Key` **solo en escrituras**. Las lecturas son públicas y no la llevan.
 */
@Injectable({ providedIn: 'root' })
export class ClubKey {
  private readonly _clave = signal<string | null>(localStorage.getItem(CLAVE_STORAGE));

  /** Clave actual (null si nadie la cargó en este navegador). */
  readonly clave = this._clave.asReadonly();

  /** ¿Hay clave guardada? El admin la usa para habilitar el formulario de carga. */
  readonly hayClave = computed(() => !!this._clave());

  guardar(clave: string): void {
    const limpia = clave.trim();
    localStorage.setItem(CLAVE_STORAGE, limpia);
    this._clave.set(limpia);
  }

  olvidar(): void {
    localStorage.removeItem(CLAVE_STORAGE);
    this._clave.set(null);
  }
}
