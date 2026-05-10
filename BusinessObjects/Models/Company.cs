using System;
using System.Collections.Generic;

namespace BusinessObjects.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
