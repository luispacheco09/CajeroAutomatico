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
                        // Validar que el último dígito no esté entre 1 y 9
                        int ultimoDigito = (int)(movimiento.Monto % 10);
                        if (ultimoDigito >= 1 && ultimoDigito <= 9)
                        {
                            result.Correct = false;
                            result.ErrorMessage = "El último dígito del monto no puede estar entre 1 y 9.";
                            return result;
                        }

                        List<int> denominaciones = CalcularDenominaciones(movimiento.Monto);

                        // Validar que solo se permitan billetes de 20, 50, 100, 200, 500 y 1000
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

        private static List<int> CalcularDenominaciones(decimal cantidad)
        {
            List<int> denominaciones = new List<int>();

            int[] valoresDenominaciones = { 1000, 500, 200, 100, 50, 20 };

            foreach (var valor in valoresDenominaciones)
            {
                int cantidadDenominacion = (int)(cantidad / valor);
                if (cantidadDenominacion > 0)
                {
                    denominaciones.Add(valor);
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
                //.Any(ad => ad.ATMDenominacion.IdAtm == 1 && ad. == denominacion && ad.Cantidad > 0);
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


        //private static void ActualizarDenominacionesEntregadas(List<int> denominaciones, DL.CajeroAutomaticoContext context, int idMovimiento)
        //{
        //    foreach (var denominacion in denominaciones)
        //    {
        //        var movimientoDenominacion = new DL.MovimientoDenominacion
        //        {
        //            IdMovimiento = idMovimiento,
        //            IdDenominacion = denominacion,
        //            Cantidad = 1
        //        };

        //        context.MovimientoDenominacions.Add(movimientoDenominacion);
        //    }

        //    context.SaveChanges();
        //}

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

        //private static void ActualizarDenominacionesEntregadas2(Dictionary<int, int> denominacionesCantidad, DL.CajeroAutomaticoContext context, int idMovimiento)
        //{
        //    // Lógica para actualizar la cantidad de billetes entregados al usuario
        //    foreach (var denominacionCantidad in denominacionesCantidad)
        //    {
        //        int denominacion = denominacionCantidad.Key;
        //        int cantidad = denominacionCantidad.Value;

        //        // Verificar que la denominacion exista en la tabla Denominacion
        //        if (context.Denominacions.Any(d => d.IdDenominacion == denominacion))
        //        {
        //            var movimientoDenominacion = new DL.MovimientoDenominacion
        //            {
        //                IdMovimiento = idMovimiento,
        //                IdDenominacion = denominacion,
        //                Cantidad = cantidad
        //            };

        //            context.MovimientoDenominacions.Add(movimientoDenominacion);
        //        }
        //        else
        //        {
        //            // Puedes manejar el error de denominación no encontrada aquí, lanzar una excepción o tomar alguna acción específica.
        //            Console.WriteLine($"La denominación con IdDenominacion {denominacion} no existe en la tabla Denominacion.");
        //        }
        //    }

        //    context.SaveChanges();
        //}

        //public static ML.Result Retiro1(DL.Movimiento movimiento)
        //{
        //    ML.Result result = new ML.Result();

        //    using (DL.CajeroAutomaticoContext context = new DL.CajeroAutomaticoContext())
        //    {
        //        try
        //        {
        //            ML.Result resulAtm = BL.Atm.GetById(1);
        //            var obj = (ML.Atm)resulAtm.Object;

        //            if (movimiento.Monto < obj.Saldo)
        //            {
        //                // Validar que el último dígito no esté entre 1 y 9
        //                int ultimoDigito = (int)(movimiento.Monto % 10);
        //                if (ultimoDigito >= 1 && ultimoDigito <= 9)
        //                {
        //                    result.Correct = false;
        //                    result.ErrorMessage = "El último dígito del monto no puede estar entre 1 y 9.";
        //                    return result;
        //                }

        //                List<int> denominaciones = CalcularDenominaciones(movimiento.Monto);

        //                // Validar que solo se permitan billetes de 20, 50, 100, 200, 500 y 1000
        //                List<int> billetesPermitidos = new List<int> { 20, 50, 100, 200, 500, 1000 };
        //                if (denominaciones.Any(den => !billetesPermitidos.Contains(den)))
        //                {
        //                    result.Correct = false;
        //                    result.ErrorMessage = "Solo se permiten billetes de 20, 50, 100, 200, 500 y 1000.";
        //                    return result;
        //                }

        //                if (!VerificarExistenciaBilletes(denominaciones, context))
        //                {
        //                    result.Correct = false;
        //                    result.ErrorMessage = "No hay suficientes billetes en el cajero para cubrir el monto solicitado";
        //                    return result;
        //                }

        //                ActualizarSaldoEnCajero(movimiento.Monto, denominaciones, context);

        //                DL.Movimiento movimientoDL = new DL.Movimiento();
        //                movimientoDL.TipoMovimiento = movimiento.TipoMovimiento;
        //                movimientoDL.Monto = movimiento.Monto;
        //                movimiento.FechaMovimiento = movimiento.FechaMovimiento;
        //                movimiento.IdCuenta = movimiento.IdCuenta;
        //                context.Add(movimientoDL);

        //                int RowsAffected = context.SaveChanges();
        //                if (RowsAffected > 0)
        //                {
        //                    ActualizarDenominacionesEntregadas1(denominaciones, context, movimientoDL.IdMovimiento);

        //                    result.Correct = true;
        //                }
        //                else
        //                {
        //                    result.Correct = false;
        //                    result.ErrorMessage = "No se pudo registrar el movimiento";
        //                }
        //            }
        //            else
        //            {
        //                result.Correct = false;
        //                result.ErrorMessage = "No hay suficiente saldo en el cajero para completar este movimiento.";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Correct = false;
        //            result.ErrorMessage = ex.Message;
        //        }

        //        return result;
        //    }
        //}


    }
}

