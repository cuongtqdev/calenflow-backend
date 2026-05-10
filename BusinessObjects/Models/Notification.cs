using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? CandidateId { get; set; }

    public int? HiringId { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? WhoSend { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual Hiring? Hiring { get; set; }
}
