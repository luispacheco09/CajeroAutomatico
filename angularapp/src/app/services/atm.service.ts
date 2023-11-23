import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Atm } from '../models/atm';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AtmService {

  //myAppUrl = 'https://localhost:7225/';
  //myApiUrl = 'api/Movimiento/';

  private apiUrl = 'https://localhost:7000';

  constructor(private http: HttpClient) { }

  getSaldoCajero(): Observable<number> {
    const url = `${this.apiUrl}/api/Atm/1`;
    return this.http.get<number>(url);
  }

  realizarRetiro(monto?: number): Observable<any> {
    const movimiento = { monto, tipoMovimiento: 'Retiro', /*fechaMovimiento: new Date(),*/ idCuenta: 1 };
    return this.http.post<any>(`${this.apiUrl}/api/Movimiento`, movimiento);
  }


}
