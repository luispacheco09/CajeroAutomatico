import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AtmService } from '../../../services/atm.service';
//import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { Atm } from '../../../models/atm';

@Component({
  selector: 'app-cajero',
  templateUrl: './cajero.component.html',
  styleUrls: ['./cajero.component.css']
})
export class CajeroComponent {
  form: FormGroup;
  suscription?: Subscription;
  producto?: Atm;
  idProducto?: number;
  Saldo?: number;
  constructor(private formBuilder: FormBuilder,
    private atmService: AtmService) {
    this.form = this.formBuilder.group({
      id: 0,
      Saldo: ['', [Validators.required]]

    })
  }

  ngOnInit() {
    this.obtenerSaldoCajero();
  }

   obtenerSaldoCajero() {
    this.atmService.getSaldoCajero().subscribe(
      (result: any) => {
        this.Saldo = result.object.saldo; 
      },
      error => {
        console.error('Error al obtener el saldo del cajero', error);
      }
    );
  }

}
