using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class HiringAvailable
{
    public int HiringAvailableId { get; set; }

    public int HiringId { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string? Status { get; set; }

    public virtual Hiring Hiring { get; set; } = null!;
}
