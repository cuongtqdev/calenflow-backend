using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Interview
{
    public int InterviewId { get; set; }

    public int CandidateId { get; set; }

    public int HiringId { get; set; }

    public string? Position { get; set; }

    public string? Type { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Time { get; set; }

    public string? Status { get; set; }

    public string? LinkMeet { get; set; }

    public string? Description { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Hiring Hiring { get; set; } = null!;
}
