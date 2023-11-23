using System;
using System.Collections.Generic;

namespace DL;

public partial class Atm
{
    public int IdAtm { get; set; }

    public decimal? Saldo { get; set; }

    public virtual ICollection<Atmdenominacion> Atmdenominacions { get; } = new List<Atmdenominacion>();
}
