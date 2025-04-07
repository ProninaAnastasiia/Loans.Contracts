namespace Loans.Contracts.Data.Models;

public class Fee
{
    public Guid FeeId { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}