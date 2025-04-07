namespace Loans.Contracts.Data.Models;

public class Pawn
{
    public Guid PawnId { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public decimal PawnAmount { get; set; }
}