using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class RentalDetail
{
    public int RentalDetailId { get; set; }

    public int? ContractId { get; set; }

    public int? EquipmentId { get; set; }

    public int? Quantity { get; set; }

    public decimal? RatePerDay { get; set; }

    public virtual RentalContract? Contract { get; set; }

    public virtual Equipment? Equipment { get; set; }
}
