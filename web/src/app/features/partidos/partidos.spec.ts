import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { Partidos } from './partidos';

describe('Partidos', () => {
  let http: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Partidos],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    http.verify();
  });

  it('pide el historial completo al entrar', () => {
    const fixture = TestBed.createComponent(Partidos);
    fixture.detectChanges();

    const pedido = http.expectOne('/api/partidos');
    expect(pedido.request.method).toBe('GET');
    pedido.flush([]);

    fixture.detectChanges();
    expect((fixture.nativeElement as HTMLElement).textContent).toContain('Todavía no hay partidos');
  });

  it('vuelve a pedir la lista al cambiar el filtro de resultado', () => {
    const fixture = TestBed.createComponent(Partidos);
    fixture.detectChanges();
    http.expectOne('/api/partidos').flush([]);
    fixture.detectChanges();

    // El segundo botón es "Ganados" (el primero es "Todos").
    const botones = fixture.nativeElement.querySelectorAll('button');
    botones[1].click();
    fixture.detectChanges();

    const pedido = http.expectOne('/api/partidos?resultado=G');
    pedido.flush([]);
    fixture.detectChanges();

    expect((fixture.nativeElement as HTMLElement).textContent).toContain(
      'No hay partidos con ese resultado',
    );
  });
});