import { Component, input } from '@angular/core';

/** Perfil de un jugador: sus stats calculadas (contrato: `GET /api/jugadores/{id}`). */
@Component({
  selector: 'app-jugador-perfil',
  imports: [],
  templateUrl: './jugador-perfil.html',
  styleUrl: './jugador-perfil.scss',
})
export class JugadorPerfil {
  /** Id del jugador, bindeado desde la ruta `/jugadores/:id`. */
  readonly id = input.required<string>();
}

