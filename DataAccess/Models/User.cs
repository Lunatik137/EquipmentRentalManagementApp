using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public string Role { get; set; } = null!;

    public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
}
