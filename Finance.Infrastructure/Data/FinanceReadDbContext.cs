using Finance.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Finance.Infrastructure.Data;

public class FinanceReadDbContext(DbContextOptions<FinanceReadDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
}