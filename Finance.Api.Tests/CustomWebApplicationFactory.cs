using Finance.Domain.Enums;
using Finance.Domain.Models;
using Finance.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static bool _databaseInitialized = false;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FinanceDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<FinanceDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = FakeAuthenticationHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = FakeAuthenticationHandler.AuthenticationScheme;
                options.DefaultScheme = FakeAuthenticationHandler.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>(
                FakeAuthenticationHandler.AuthenticationScheme, _ => { });
        });
    }

    public async Task InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_databaseInitialized)
                return;

            _databaseInitialized = true;

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();

            var user = new User { Id = 1, Name = "Test User", Email = "test@user.com", PasswordHash = "any_hash" };
            await dbContext.Users.AddAsync(user);

            var category = new Category { Id = 1, Title = "Alimentação", Description = "Gastos com Comida", UserId = user.Id };
            await dbContext.Categories.AddAsync(category);

            await dbContext.Transactions.AddAsync(new Transaction
            {
                Id = 1,
                Title = "Jantar de Teste",
                Amount = 150,
                CreatedAt = DateTime.Now,
                PaidOrReceivedAt = DateTime.Now,
                Type = ETransactionType.Withdraw,
                CategoryId = category.Id,
                UserId = user.Id
            });

            await dbContext.SaveChangesAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
    }
}