using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class PRESCRIPTION
{
    public decimal PRESCRIPTION_ID { get; set; }

    public decimal APPOINTMENT_ID { get; set; }

    public string? CONTENT { get; set; }

    public DateTime CREATED_AT { get; set; }

    public virtual APPOINTMENT APPOINTMENT { get; set; } = null!;
}
