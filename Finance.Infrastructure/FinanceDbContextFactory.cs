using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Finance.Infrastructure.Data;

namespace Finance.Infrastructure
{
    public class FinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
    {
        public FinanceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FinanceDbContext>();

            optionsBuilder.UseSqlServer("Server=DESKTOP-M76AS7V\\ALYSONSZ; Database=Finance; User Id=EventUser; Password=12345; TrustServerCertificate=True;");

            return new FinanceDbContext(optionsBuilder.Options);
        }
    }
}
