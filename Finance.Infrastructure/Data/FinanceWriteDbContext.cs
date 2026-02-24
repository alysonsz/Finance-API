using Finance.Domain.Models;
using Finance.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data;

public class FinanceWriteDbContext(DbContextOptions<FinanceWriteDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryMapping());
        modelBuilder.ApplyConfiguration(new TransactionMapping());
        modelBuilder.ApplyConfiguration(new UserMapping());
        modelBuilder.ApplyConfiguration(new OutboxMessageMapping());

        base.OnModelCreating(modelBuilder);
    }
}
