import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { Home } from './home';

const CLUB = {
  discordUrl: 'https://discord.gg/elpodefc',
  horarioJuego: '22:00:00',
  diasJuego: ['vie'],
  pausado: false,
  juegaHoy: true,
  diaDeHoy: 'vie',
};

const PARTIDOS = [
  {
    id: 1,
    fecha: '2026-09-18T01:00:00+00:00',
    rival: 'Los Elefantes FC',
    resultado: 'G',
    marcadorPropio: 4,
    marcadorRival: 2,
    tipo: 'Liga',
    notas: null,
    registradoPor: 'Lean',
    cantidadMedios: 1,
  },
];

describe('Home', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  it('muestra que hoy se juega, el horario, el Discord y los últimos resultados', () => {
    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();

    http.expectOne('/api/club').flush(CLUB);
    http.expectOne('/api/partidos?limit=5').flush(PARTIDOS);
    fixture.detectChanges();

    const html = fixture.nativeElement as HTMLElement;

    expect(html.textContent).toContain('Hoy se juega');
    expect(html.textContent).toContain('22:00');
    expect(html.textContent).toContain('Los Elefantes FC');
    expect(html.textContent).toContain('4 - 2');
    expect(html.querySelector('a[href="https://discord.gg/elpodefc"]')).toBeTruthy();
  });

  it('si el club falla, avisa y deja reintentar', () => {
    const fixture = TestBed.createComponent(Home);
    fixture.detectChanges();

    http.expectOne('/api/club').error(new ProgressEvent('error'));
    http.expectOne('/api/partidos?limit=5').flush(PARTIDOS);
    fixture.detectChanges();

    const html = fixture.nativeElement as HTMLElement;

    expect(html.textContent).toContain('No pudimos cargar los datos del club');
    expect(html.querySelector('button')?.textContent).toContain('Reintentar');
  });
});