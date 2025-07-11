using System;
using System.Collections.Generic;

namespace DataAccess.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? ContractId { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public decimal? AmountPaid { get; set; }

    public string? Note { get; set; }

    public virtual RentalContract? Contract { get; set; }
}
