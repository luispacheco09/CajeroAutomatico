import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Atm } from '../models/atm';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Subscription } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AtmService {

  private apiUrl = 'https://localhost:7000';
  Saldo?: number;
  subscription?: Subscription;


  private saldoSubject = new BehaviorSubject<number>(0);

  private denominacionesSubject = new BehaviorSubject<number[]>([]);


  constructor(private http: HttpClient) { }

  getSaldoCajero(): Observable<number> {
    const url = `${this.apiUrl}/api/Atm/1`;
    return this.http.get<number>(url).pipe(
      tap(saldo => {
        console.log('Nuevo saldo emitido:', saldo);
        this.saldoSubject.next(saldo);
      })
    );
  }

  observeSaldoCajero(): Observable<number> {
    return this.saldoSubject.asObservable();
  }

  realizarRetiro(monto?: number): Observable<any> {
    const movimiento = { monto, tipoMovimiento: 'Retiro', idCuenta: 1 };
    return this.http.post<any>(`${this.apiUrl}/api/Movimiento`, movimiento).pipe(
      tap(result => {
        this.denominacionesSubject.next(result.denominaciones);
        this.getSaldoCajero();
      })
    );
  }
  obtenerDenominacionesEntregadas(idMovimiento: number): Observable<number[]> {
    const url = `${this.apiUrl}/api/MovimientoDenominacion/${idMovimiento}`;
    return this.http.get<number[]>(url);
  }

  actualizarSaldoCajero() {
    this.getSaldoCajero().subscribe(
      (result: any) => {
        this.saldoSubject.next(result.object.saldo);
      },
      error => {
        console.error('Error al obtener el saldo del cajero', error);
      }
    );
  }
}

