using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class CLINIC
{
    public decimal CLINIC_ID { get; set; }

    public string NAME { get; set; } = null!;

    public string? ADDRESS { get; set; }

    public string? PHONE { get; set; }

    public virtual ICollection<DOCTOR> DOCTORs { get; set; } = new List<DOCTOR>();
}
