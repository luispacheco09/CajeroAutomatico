using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class MovimientoDenominacion
    {
        public static ML.Result MovimientoDenominaciones(DL.MovimientoDenominacion movimientoDenominacion, int? monto)
        {
            ML.Result result = new ML.Result();

            using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
            {
                try
                {
                    DL.MovimientoDenominacion movimientoDL = new DL.MovimientoDenominacion();

                    if (monto != null)
                    {
                        int? cantidad = 0;
                        cantidad = monto / 1000;
                        if (cantidad > 0 & cantidad != '.')
                        {
                            movimientoDL.Cantidad = cantidad;
                            movimientoDL.IdDenominacion = 1;
                        }
                        else
                        {
                            cantidad = monto / 500;
                            if (cantidad > 0 & cantidad != '.')
                            {
                                movimientoDL.Cantidad = cantidad;
                                movimientoDL.IdDenominacion = 2;
                            }
                        }
                    }

                    movimientoDL.IdMovimiento = movimientoDenominacion.IdMovimiento;
                    //
                    //movimientoDL.IdDenominacion = movimientoDenominacion.IdDenominacion;
                    //

                    //movimientoDL.Cantidad = movimientoDenominacion.Cantidad;



                    context.Add(movimientoDL);

                    int RowsAffected = context.SaveChanges();
                    if (RowsAffected > 0)
                    {
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se pudo registrar el movimiento";
                    }


                }
                catch (Exception ex)
                {
                    result.Correct = false;
                    result.ErrorMessage = ex.Message;
                }
            }
            return result;
        }
    }
}
