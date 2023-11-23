using DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class MovimientoDenominacion
    {
        public static ML.Result MovimientoDenominaciones(int IdMovimiento )
        {
            ML.Result result = new ML.Result();

            using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
            {
                try
                {

                    var listaMD = (from movimientoDenominacionDL in context.MovimientoDenominacions
                                 where movimientoDenominacionDL.IdMovimiento == IdMovimiento
                                 select new
                                 {
                                     IdMovimientoDenominacion = movimientoDenominacionDL.IdMovimientoDenominacion,
                                     IdMovimiento = movimientoDenominacionDL.IdMovimiento,
                                     IdDenominacion = movimientoDenominacionDL.IdDenominacion,
                                     Cantidad = movimientoDenominacionDL.Cantidad
                                 }).ToList();
                    if (listaMD != null && listaMD.Count > 0)
                    {
                        result.Objects = new List<object>();
                        foreach (var obj in listaMD)
                        {
                            DL.MovimientoDenominacion movimientoDenominacion = new DL.MovimientoDenominacion();
                            movimientoDenominacion.IdMovimientoDenominacion = obj.IdMovimientoDenominacion;
                            movimientoDenominacion.IdMovimiento = obj.IdMovimiento;
                            movimientoDenominacion.IdDenominacion = obj.IdDenominacion;
                            movimientoDenominacion.Cantidad = obj.Cantidad;

                            result.Objects.Add(movimientoDenominacion);
                        }
                        result.Correct = true;
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No se encontraron los datos de esos movimientos";
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
