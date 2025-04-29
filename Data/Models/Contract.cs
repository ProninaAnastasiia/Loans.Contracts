namespace Loans.Contracts.Data.Models;

public class Contract
{
    public Guid ContractId { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid ClientId { get; set; }
    public DateTime LodgementDate { get; set; }
    public Guid DecisionId { get; set; }
    public string CreditProductId { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanTermMonths { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanPurpose { get; set; }
    public string LoanType { get; set; }
    public string PaymentType { get; set; }
    public decimal? InitialPaymentAmount { get; set; }
    public Pawn? Pawn { get; set; }
    public Insurance? Insurance { get; set; }
    
    public Guid OperationId { get; set; }
    public Guid? CloseOperationId { get; set; }
    public Guid? ActivateOperationId { get; set; }
    
    public DateTime? BeginDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public string ContractStatus { get; set; }
    public Guid? CreditIssuanceAccount { get; set; }
    public Guid? DebitFromAccount { get; set; }
    public decimal? MonthlyPaymentAmount { get; set; } // Сумма основного долга
    public decimal? TotalPaymentAmount { get; set; } // Общая сумма денежных средств, которую заемщик выплатит кредитору за весь срок кредита.
    public decimal? TotalInterestPaid { get; set; } // Все проценты, начисленные за весь срок кредита 
    public decimal? FullLoanValue { get; set; } // ПСК - полная стоимость кредита. Это процентная ставка, выраженная в годовых процентах, которая отражает все расходы заёмщика, связанные с получением и обслуживанием кредита.
    public Guid? PaymentScheduleId { get; set; }
    public string AdditionalTerms { get; set; }
    public DateTime LastModificationDate { get; set; }
    
    public List<Fee?> Fees { get; set; }
}