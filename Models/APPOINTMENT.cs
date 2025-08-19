using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class APPOINTMENT
{
    public decimal APPOINTMENT_ID { get; set; }

    public decimal PATIENT_ID { get; set; }

    public decimal DOCTOR_ID { get; set; }

    public decimal? SCHEDULE_ID { get; set; }

    public DateTime SCHEDULED_AT { get; set; }

    public string STATUS { get; set; } = null!;

    public DateTime CREATED_AT { get; set; }

    public DateTime? UPDATED_AT { get; set; }

    public string? NOTES { get; set; }

    public virtual DOCTOR DOCTOR { get; set; } = null!;

    public virtual PATIENT PATIENT { get; set; } = null!;

    public virtual ICollection<PAYMENT> PAYMENTs { get; set; } = new List<PAYMENT>();

    public virtual ICollection<PRESCRIPTION> PRESCRIPTIONs { get; set; } = new List<PRESCRIPTION>();

    public virtual SCHEDULE? SCHEDULE { get; set; }
}
