using Loans.Contracts.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Loans.Contracts.Data;

public class LoanContractContext: DbContext
{
    public LoanContractContext(DbContextOptions<LoanContractContext> options) : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; }
}