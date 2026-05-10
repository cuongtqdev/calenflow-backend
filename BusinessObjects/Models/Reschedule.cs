using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Reschedule
{
    public int HiringId { get; set; }

    public int CandidateId { get; set; }

    public DateTime OrginalDate { get; set; }

    public DateTime RescheduleDate { get; set; }

    public string? Reason { get; set; }

    public bool? IsAccept { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Hiring Hiring { get; set; } = null!;
}
