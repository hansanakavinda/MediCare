using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class PAYMENT
{
    public decimal PAYMENT_ID { get; set; }

    public decimal APPOINTMENT_ID { get; set; }

    public decimal AMOUNT { get; set; }

    public string? METHOD { get; set; }

    public string? STATUS { get; set; }

    public DateTime? PAID_AT { get; set; }

    public string? TXN_REF { get; set; }

    public virtual APPOINTMENT APPOINTMENT { get; set; } = null!;
}
