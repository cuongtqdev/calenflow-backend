using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Candidate
{
    public int CandidateId { get; set; }

    public string? UrlCv { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CandidateInvite> CandidateInvites { get; set; } = new List<CandidateInvite>();

    public virtual User CandidateNavigation { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Reschedule> Reschedules { get; set; } = new List<Reschedule>();
}
