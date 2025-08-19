using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class DOCTOR
{
    public decimal DOCTOR_ID { get; set; }

    public decimal USER_ID { get; set; }

    public decimal? SPECIALTY_ID { get; set; }

    public decimal? CLINIC_ID { get; set; }

    public string? BIO { get; set; }

    public decimal? FEE { get; set; }

    public virtual ICollection<APPOINTMENT> APPOINTMENTs { get; set; } = new List<APPOINTMENT>();

    public virtual CLINIC? CLINIC { get; set; }

    public virtual ICollection<FEEDBACK> FEEDBACKs { get; set; } = new List<FEEDBACK>();

    public virtual ICollection<SCHEDULE> SCHEDULEs { get; set; } = new List<SCHEDULE>();

    public virtual SPECIALTY? SPECIALTY { get; set; }

    public virtual USER USER { get; set; } = null!;
}
