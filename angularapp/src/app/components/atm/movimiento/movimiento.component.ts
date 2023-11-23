import { Component } from '@angular/core';
import { AtmService } from '../../../services/atm.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-movimiento',
  templateUrl: './movimiento.component.html',
  styleUrls: ['./movimiento.component.css']
})
export class MovimientoComponent {
  monto?: number;
  denominaciones?: number[];
  constructor(private retiroService: AtmService,
    private toastr: ToastrService) { }

  realizarRetiro() {
    this.retiroService.realizarRetiro(this.monto)
      .subscribe(result => {
        if (result.correct) {
          //console.log('Retiro exitoso');
          this.toastr.success('Retiro exitoso', 'El retiro fue realizado correctamente');
          this.retiroService.getSaldoCajero();
          this.denominaciones = result.denominaciones;
        }
        else {
          //console.error('Error en el retiro:', response.errorMessage);
          this.toastr.warning('Error en el retiro:', result.ErrorMessage);
        }
      });
  }
}
