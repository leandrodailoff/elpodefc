/**
 * Ayudas para los links de video.
 *
 * A YouTube se lo muestra **embebido** (se ve en la página, sin salir del sitio);
 * el resto de los videos y las fotos se abren en una pestaña nueva.
 */

/** Saca el id de un link de YouTube, o null si el link no es de YouTube. */
export function youtubeId(url: string | null): string | null {
  if (!url) return null;

  // Formatos que se usan en la práctica: youtu.be/ID · watch?v=ID · embed/ID · shorts/ID
  const patrones = [
    /youtu\.be\/([\w-]{6,})/i,
    /[?&]v=([\w-]{6,})/i,
    /youtube\.com\/embed\/([\w-]{6,})/i,
    /youtube\.com\/shorts\/([\w-]{6,})/i,
  ];

  for (const patron of patrones) {
    const encontrado = patron.exec(url);
    if (encontrado) return encontrado[1];
  }

  return null;
}

/** Lo que ve la galería: la miniatura que da YouTube sin pedir nada más. */
export function youtubeMiniatura(id: string): string {
  return `https://img.youtube.com/vi/${id}/mqdefault.jpg`;
}

/** El reproductor embebido (sin cookies hasta que se aprieta play: es un `<iframe>` de YouTube). */
export function youtubeEmbed(url: string | null): string | null {
  const id = youtubeId(url);
  return id ? `https://www.youtube.com/embed/${id}` : null;
}