using System;
using System.Collections.Generic;

namespace DL;

public partial class Movimiento
{
    public int IdMovimiento { get; set; }

    public string? TipoMovimiento { get; set; }

    public int Monto { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public int? IdCuenta { get; set; }

    public virtual Cuentum? IdCuentaNavigation { get; set; }

    public virtual ICollection<MovimientoDenominacion> MovimientoDenominacions { get; } = new List<MovimientoDenominacion>();
}
