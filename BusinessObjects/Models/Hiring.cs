using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Hiring
{
    public int HiringId { get; set; }

    public string? Position { get; set; }

    public virtual ICollection<CandidateInvite> CandidateInvites { get; set; } = new List<CandidateInvite>();

    public virtual ICollection<HiringAvailable> HiringAvailables { get; set; } = new List<HiringAvailable>();

    public virtual User HiringNavigation { get; set; } = null!;

    public virtual ICollection<Interview> Interviews { get; set; } = new List<Interview>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Reschedule> Reschedules { get; set; } = new List<Reschedule>();
}
