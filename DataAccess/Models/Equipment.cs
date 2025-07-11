using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? Status { get; set; }

    public int? QuantityAvailable { get; set; }

    public decimal? DailyRate { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public virtual ICollection<RentalDetail> RentalDetails { get; set; } = new List<RentalDetail>();
}
