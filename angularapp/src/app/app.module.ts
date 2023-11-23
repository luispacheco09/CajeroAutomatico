import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms'
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app.component';
import { ATMComponent } from './components/atm/atm.component';
import { CajeroComponent } from './components/atm/cajero/cajero.component';
import { MovimientoComponent } from './components/atm/movimiento/movimiento.component';

@NgModule({
  declarations: [
    AppComponent,
    ATMComponent,
    CajeroComponent,
    MovimientoComponent,

  ],
  imports: [
    BrowserModule, HttpClientModule,
    ReactiveFormsModule,
    BrowserAnimationsModule, // required animations module
    ToastrModule.forRoot(), // ToastrModule added
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
