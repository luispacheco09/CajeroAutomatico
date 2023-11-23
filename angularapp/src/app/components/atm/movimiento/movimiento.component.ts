import { AtmService } from '../../../services/atm.service';
import { ToastrService } from 'ngx-toastr';
import { Component, NgZone } from '@angular/core';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-movimiento',
  templateUrl: './movimiento.component.html',
  styleUrls: ['./movimiento.component.css']
})
export class MovimientoComponent {
  monto?: number;
  denominaciones?: number[];
  Saldo?: number;

  constructor(private retiroService: AtmService,
    private toastr: ToastrService,
    private zone: NgZone) { }

  realizarRetiro() {
    this.retiroService.realizarRetiro(this.monto)
      .subscribe(
        result => {
          if (result.correct) {
            this.mostrarMensaje('success', 'Retiro exitoso', 'El retiro fue realizado correctamente');

            this.obtenerDenominaciones(result.objects[0].idMovimiento);

            this.zone.run(() => {
              this.retiroService.actualizarSaldoCajero();
            });
          } else {
            this.mostrarMensaje('warning', 'Error en el retiro:', result.errorMessage || 'Error desconocido');
          }
        },
        error => {
          console.error('Error al realizar retiro:', error.error);
          this.mostrarMensaje('warning', 'Error en el retiro:', error.error.errorMessage || 'Error desconocido');
        }
      );
  }
  private obtenerDenominaciones(idMovimiento: number) {
    this.retiroService
      .obtenerDenominacionesEntregadas(idMovimiento)
      .subscribe((denominaciones) => {
        this.denominaciones = denominaciones;
      });
  }
  private mostrarMensaje(type: 'success' | 'warning', title: string, message: string) {
    if (type === 'success') {
      this.toastr.success(title, message);
    } else {
      this.toastr.warning(title, message);
    }
  }
}
