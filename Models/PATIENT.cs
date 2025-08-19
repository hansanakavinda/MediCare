using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class PATIENT
{
    public decimal PATIENT_ID { get; set; }

    public decimal USER_ID { get; set; }

    public DateTime? DOB { get; set; }

    public string? GENDER { get; set; }

    public string? ADDRESS { get; set; }

    public virtual ICollection<APPOINTMENT> APPOINTMENTs { get; set; } = new List<APPOINTMENT>();

    public virtual ICollection<FEEDBACK> FEEDBACKs { get; set; } = new List<FEEDBACK>();

    public virtual USER USER { get; set; } = null!;
}
