using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Finance.Infrastructure.Data;

namespace Finance.Infrastructure;

public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();

        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=FinanceDb;Trusted_Connection=True;");

        return new FinanceDbContext(optionsBuilder.Options);
    }
}
