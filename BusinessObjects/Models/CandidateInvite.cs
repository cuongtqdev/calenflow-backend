using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class CandidateInvite
{
    public int Id { get; set; }

    public int CandidateId { get; set; }

    public int HiringId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpireAt { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Candidate Candidate { get; set; } = null!;

    public virtual Hiring Hiring { get; set; } = null!;
}
