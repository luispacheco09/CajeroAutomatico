using System;
using System.Collections.Generic;

namespace DL;

public partial class MovimientoDenominacion
{
    public int IdMovimientoDenominacion { get; set; }

    public int? IdMovimiento { get; set; }

    public int? IdDenominacion { get; set; }

    public int? Cantidad { get; set; }

    public virtual Denominacion? IdDenominacionNavigation { get; set; }

    public virtual Movimiento? IdMovimientoNavigation { get; set; }
}
