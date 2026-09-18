import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { ClubKey } from './club-key';

/**
 * Agrega el header `X-Club-Key` en las escrituras (POST/PUT/DELETE).
 *
 * Las lecturas (GET) son públicas: nunca llevan la clave. Si no hay clave guardada,
 * la petición sale tal cual y el backend responde 401 (el admin pide la clave entonces).
 */
export const clubKeyInterceptor: HttpInterceptorFn = (req, next) => {
  const clave = inject(ClubKey).clave();

  if (!clave || req.method === 'GET') {
    return next(req);
  }

  return next(req.clone({ setHeaders: { 'X-Club-Key': clave } }));
};
