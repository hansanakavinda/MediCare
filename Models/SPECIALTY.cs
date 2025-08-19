using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class SPECIALTY
{
    public decimal SPECIALTY_ID { get; set; }

    public string NAME { get; set; } = null!;

    public virtual ICollection<DOCTOR> DOCTORs { get; set; } = new List<DOCTOR>();
}
