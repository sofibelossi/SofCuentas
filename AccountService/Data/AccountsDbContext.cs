using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data;

public class AccountsDbContext(DbContextOptions<AccountsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Movement> Movements => Set<Movement>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Account>(e =>
        {
            e.Property(a => a.Owner).HasMaxLength(100).IsRequired();
            e.Property(a => a.Balance).HasPrecision(18, 2);
            e.Property(a => a.RowVersion).IsRowVersion();
        });
        b.Entity<Movement>(e =>
        {
            e.Property(m => m.Amount).HasPrecision(18, 2);
            e.Property(m => m.Description).HasMaxLength(200);
            e.HasOne<Account>().WithMany(a => a.Movements).HasForeignKey(m => m.AccountId);
        });
    }
}