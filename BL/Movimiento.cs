using DL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class Movimiento
    {
        private readonly CajeroAutomaticoContext _context;

        public Movimiento(CajeroAutomaticoContext context)
        {
            _context = context;
        }

        public static ML.Result Retiro(DL.Movimiento movimiento)
        {
            ML.Result result = new ML.Result();

            using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
            {
                try
                {
                    ML.Result resulAtm = BL.Atm.GetById(1);
                    var obj = (ML.Atm)resulAtm.Object;

                    if (movimiento.Monto < obj.Saldo)
                    {
                        int ultimoDigito = (int)(movimiento.Monto % 10);
                        if (ultimoDigito >= 1 && ultimoDigito <= 9)
                        {
                            result.Correct = false;
                            result.ErrorMessage = "El último dígito del monto no puede estar entre 1 y 9.";
                            return result;
                        }

                        List<int> denominaciones = CalcularDenominaciones(movimiento.Monto);

                        List<int> billetesPermitidos = new List<int> { 20, 50, 100, 200, 500, 1000 };
                        if (denominaciones.Any(den => !billetesPermitidos.Contains(den)))
                        {
                            result.Correct = false;
                            result.ErrorMessage = "Solo se permiten billetes de 20, 50, 100, 200, 500 y 1000.";
                            return result;
                        }

                        if (!VerificarExistenciaBilletes(denominaciones, context))
                        {
                            result.ErrorMessage = "No hay suficientes billetes en el cajero para cubrir el monto solicitado";
                            return result;
                        }

                        ActualizarSaldoEnCajero(movimiento.Monto, denominaciones, context);

                        DL.Movimiento movimientoDL = new DL.Movimiento();
                        movimientoDL.TipoMovimiento = movimiento.TipoMovimiento;
                        movimientoDL.Monto = movimiento.Monto;
                        movimiento.FechaMovimiento = DateTime.Now;
                        movimiento.IdCuenta = movimiento.IdCuenta;
                        context.Add(movimientoDL);

                        int RowsAffected = context.SaveChanges();
                        if (RowsAffected > 0)
                        {
                            ActualizarDenominacionesEntregadas1(denominaciones, context, movimientoDL.IdMovimiento);

                            result.Correct = true;

                            ML.Result resulmd = BL.MovimientoDenominacion.MovimientoDenominaciones(movimientoDL.IdMovimiento);
                            var listaDenominaciones = (List<object>)resulmd.Objects;

                            result.Objects = listaDenominaciones?.Cast<object>().ToList();



                        }
                        else
                        {
                            result.Correct = false;
                            result.ErrorMessage = "No se pudo registrar el movimiento";
                        }
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No hay suficiente saldo en el cajero para completar este movimiento.";
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

        private static List<int> CalcularDenominaciones(decimal cantidad)
        {
            List<int> denominaciones = new List<int>();

            int[] valoresDenominaciones = { 1000, 500, 200, 100, 50, 20 };

            foreach (var valor in valoresDenominaciones)
            {
                int cantidadDenominacion = (int)(cantidad / valor);
                if (cantidadDenominacion > 0)
                {

                    for (int i = 0; i < cantidadDenominacion; i++)
                    {
                        denominaciones.Add(valor);
                    }

                    cantidad -= cantidadDenominacion * valor;
                }
            }

            return denominaciones;
        }



        private static bool VerificarExistenciaBilletes(List<int> denominaciones, DL.CajeroAutomaticoContext context)
        {
            foreach (var denominacion in denominaciones)
            {
                var existencia = context.Atmdenominacions
                    .Join(
                        context.Denominacions,
                        ad => ad.IdDenominacion,
                        d => d.IdDenominacion,
                        (ad, d) => new { ATMDenominacion = ad, Denominacion = d }
                    )
                .Any(joined => joined.ATMDenominacion.IdAtm == 1 && joined.ATMDenominacion.Cantidad > 0 &&
                       denominaciones.Contains((int)joined.Denominacion.Denominación));

                if (!existencia)
                {
                    return false;
                }
                return true;

            }
            return false;
        }

        private static void ActualizarSaldoEnCajero(decimal monto, List<int> denominaciones, DL.CajeroAutomaticoContext context)
        {
            var cajero = context.Atms.Find(1);
            cajero.Saldo -= monto;

            foreach (var denominacion in denominaciones)
            {
                var denominacionEnCajero = context.Atmdenominacions
                    .Join(
                        context.Denominacions,
                        ad => ad.IdDenominacion,
                        d => d.IdDenominacion,
                        (ad, d) => new { ATMDenominacion = ad, Denominacion = d }
                    )
                    .FirstOrDefault(joined => joined.ATMDenominacion.IdAtm == 1 &&
                                                 joined.Denominacion.Denominación == denominacion);

                if (denominacionEnCajero != null)
                {
                    denominacionEnCajero.ATMDenominacion.Cantidad -= 1;
                }
            }

            context.SaveChanges();
        }

        private static void ActualizarDenominacionesEntregadas1(List<int> denominaciones, DL.CajeroAutomaticoContext context, int idMovimiento)
        {
            var denominacionesExistentes = context.Denominacions
                .Where(d => denominaciones.Contains((int)d.Denominación))
                .Select(d => d.IdDenominacion)
                .ToList();

            var movimientoDenominaciones = denominacionesExistentes
                .Select(IdDenominacion => new DL.MovimientoDenominacion
                {
                    IdMovimiento = idMovimiento,
                    IdDenominacion = IdDenominacion,
                    Cantidad = 1
                })
                .ToList();

            context.MovimientoDenominacions.AddRange(movimientoDenominaciones);
            context.SaveChanges();
        }

        public static ML.Result MovimientoDenominacion(DL.Movimiento movimiento)
        {
            ML.Result result = new ML.Result();

            using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
            {
                try
                {
                    ML.Result resulAtm = BL.Atm.GetById(1);
                    var obj = (ML.Atm)resulAtm.Object;

                    if (movimiento.Monto < obj.Saldo)
                    {
                        int ultimoDigito = (int)(movimiento.Monto % 10);
                        if (ultimoDigito >= 1 && ultimoDigito <= 9)
                        {
                            result.Correct = false;
                            result.ErrorMessage = "El último dígito del monto no puede estar entre 1 y 9.";
                            return result;
                        }

                        List<int> denominaciones = CalcularDenominaciones(movimiento.Monto);

                        List<int> billetesPermitidos = new List<int> { 20, 50, 100, 200, 500, 1000 };
                        if (denominaciones.Any(den => !billetesPermitidos.Contains(den)))
                        {
                            result.Correct = false;
                            result.ErrorMessage = "Solo se permiten billetes de 20, 50, 100, 200, 500 y 1000.";
                            return result;
                        }

                        if (!VerificarExistenciaBilletes(denominaciones, context))
                        {
                            result.ErrorMessage = "No hay suficientes billetes en el cajero para cubrir el monto solicitado";
                            return result;
                        }

                        ActualizarSaldoEnCajero(movimiento.Monto, denominaciones, context);

                        DL.Movimiento movimientoDL = new DL.Movimiento();
                        movimientoDL.TipoMovimiento = movimiento.TipoMovimiento;
                        movimientoDL.Monto = movimiento.Monto;
                        movimiento.FechaMovimiento = DateTime.Now;
                        movimiento.IdCuenta = movimiento.IdCuenta;
                        context.Add(movimientoDL);

                        int RowsAffected = context.SaveChanges();
                        if (RowsAffected > 0)
                        {
                            ActualizarDenominacionesEntregadas1(denominaciones, context, movimientoDL.IdMovimiento);

                            result.Correct = true;
                        }
                        else
                        {
                            result.Correct = false;
                            result.ErrorMessage = "No se pudo registrar el movimiento";
                        }
                    }
                    else
                    {
                        result.Correct = false;
                        result.ErrorMessage = "No hay suficiente saldo en el cajero para completar este movimiento.";
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

