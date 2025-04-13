using Loans.Contracts.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Loans.Contracts.Data;

public class LoanContractDbContext: DbContext
{
    public LoanContractDbContext(DbContextOptions<LoanContractDbContext> options) : base(options)
    {
    }

    public DbSet<Contract> Contracts { get; set; }
    public DbSet<Fee> Fees { get; set; }
    public DbSet<Insurance> Insurances { get; set; }
    public DbSet<Pawn> Pawns { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>().HasKey(u => u.ContractId);
        modelBuilder.Entity<Fee>().HasKey(u => u.FeeId);
        modelBuilder.Entity<Insurance>().HasKey(u => u.InsuranceId);
        modelBuilder.Entity<Pawn>().HasKey(u => u.PawnId);
        
        base.OnModelCreating(modelBuilder);
    }
}