using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class User
{
    public int UserId { get; set; }

    public int CompanyId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? UserName { get; set; }

    public string? PasswordHash { get; set; }

    public string Role { get; set; } = null!;

    public string? Bio { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Candidate? Candidate { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual Hiring? Hiring { get; set; }
}
