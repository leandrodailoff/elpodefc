/**
 * Ayudas para mostrar el horario del club.
 *
 * Los días viajan como abreviatura (`vie`) porque es lo que guarda la BD; acá se
 * traducen a algo que se pueda leer en pantalla.
 */

export const NOMBRES_DIA: Record<string, string> = {
  dom: 'Domingo',
  lun: 'Lunes',
  mar: 'Martes',
  mie: 'Miércoles',
  jue: 'Jueves',
  vie: 'Viernes',
  sab: 'Sábado',
};

/** "mar, mie, vie" → "Martes, Miércoles y Viernes". */
export function textoDias(dias: string[]): string {
  const nombres = dias.map((d) => NOMBRES_DIA[d] ?? d);
  if (nombres.length <= 1) return nombres.join('');

  return `${nombres.slice(0, -1).join(', ')} y ${nombres[nombres.length - 1]}`;
}

/** "22:00:00" → "22:00" (el segundo no le interesa a nadie). */
export function textoHora(hora: string | null): string {
  return hora ? hora.slice(0, 5) : '';
}
