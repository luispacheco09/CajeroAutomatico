using System;
using System.Collections.Generic;

namespace DL;

public partial class Atmdenominacion
{
    public int IdAtmdenominacion { get; set; }

    public int? IdAtm { get; set; }

    public int? IdDenominacion { get; set; }

    public int? Cantidad { get; set; }

    public virtual Atm? IdAtmNavigation { get; set; }

    public virtual Denominacion? IdDenominacionNavigation { get; set; }
}
