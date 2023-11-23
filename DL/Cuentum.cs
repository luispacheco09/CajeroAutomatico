using System;
using System.Collections.Generic;

namespace DL;

public partial class Cuentum
{
    public int IdCuenta { get; set; }

    public string? Nombre { get; set; }

    public int? NumeroCuenta { get; set; }

    public decimal? Saldo { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; } = new List<Movimiento>();
}
