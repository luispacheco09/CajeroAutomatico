using System;
using System.Collections.Generic;

namespace DL;

public partial class Denominacion
{
    public int IdDenominacion { get; set; }

    public int? Denominación { get; set; }

    public virtual ICollection<Atmdenominacion> Atmdenominacions { get; } = new List<Atmdenominacion>();

    public virtual ICollection<MovimientoDenominacion> MovimientoDenominacions { get; } = new List<MovimientoDenominacion>();
}
