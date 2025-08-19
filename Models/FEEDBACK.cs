using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class FEEDBACK
{
    public decimal FEEDBACK_ID { get; set; }

    public decimal PATIENT_ID { get; set; }

    public decimal DOCTOR_ID { get; set; }

    public bool RATING { get; set; }

    public string? MSG { get; set; }

    public DateTime CREATED_AT { get; set; }

    public virtual DOCTOR DOCTOR { get; set; } = null!;

    public virtual PATIENT PATIENT { get; set; } = null!;
}
