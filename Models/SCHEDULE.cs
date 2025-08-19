using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class SCHEDULE
{
    public decimal SCHEDULE_ID { get; set; }

    public decimal DOCTOR_ID { get; set; }

    public bool DAY_OF_WEEK { get; set; }

    public string START_TIME { get; set; } = null!;

    public string END_TIME { get; set; } = null!;

    public byte? SLOT_MINUTES { get; set; }

    public virtual ICollection<APPOINTMENT> APPOINTMENTs { get; set; } = new List<APPOINTMENT>();

    public virtual DOCTOR DOCTOR { get; set; } = null!;
}
