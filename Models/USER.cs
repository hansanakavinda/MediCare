using System;
using System.Collections.Generic;

namespace MediCare.Models;

public partial class USER
{
    public decimal USER_ID { get; set; }

    public string EMAIL { get; set; } = null!;

    public string PWD { get; set; } = null!;

    public string FULL_NAME { get; set; } = null!;

    public string? PHONE { get; set; }

    public string ROLE { get; set; } = null!;

    public string IS_ACTIVE { get; set; } = null!;

    public DateTime CREATED_AT { get; set; }

    public virtual DOCTOR? DOCTOR { get; set; }

    public virtual PATIENT? PATIENT { get; set; }
}
