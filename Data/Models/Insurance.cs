﻿namespace Loans.Contracts.Data.Models;

public class Insurance
{
    public Guid InsuranceId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public string Company { get; set; }
}