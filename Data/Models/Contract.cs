namespace Loans.Contracts.Data.Models;

public class Contract
{
    public Guid ContractId { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime LodgementDate { get; set; }
    public Guid DecisionId { get; set; }
    public string CreditProductId { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime? DisbursementDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public Guid OperationId { get; set; }
    public Guid? CloseOperationId { get; set; }
    public Guid? ActivateOperationId { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanTermMonths { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanPurpose { get; set; }
    public string LoanType { get; set; }
    public string PaymentType { get; set; }
    public decimal? InitialPaymentAmount { get; set; }
    public Pawn Pawn { get; set; }
    public Insurance Insurance { get; set; }
    public string ContractStatus { get; set; }
    public Guid CreditIssuanceAccount { get; set; }
    public Guid DebitFromAccount { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; }
    public decimal? TotalPaymentAmount { get; set; }
    public decimal? TotalInterestPaid { get; set; }
    public decimal? Psk { get; set; }
    public Guid? PaymentScheduleId { get; set; }
    public string AdditionalTerms { get; set; }
    public DateTime LastModificationDate { get; set; }
    
    public List<Fee> Fees { get; set; }
}