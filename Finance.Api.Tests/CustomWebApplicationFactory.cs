using Finance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace Finance.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    private readonly RedisContainer _redisContainer = new RedisBuilder("redis:alpine")
        .Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mssqlContainer.StartAsync(), _redisContainer.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(_mssqlContainer.StopAsync(), _redisContainer.StopAsync());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<FinanceDbContext>));

            services.AddDbContext<FinanceDbContext>(options =>
            {
                options.UseSqlServer(_mssqlContainer.GetConnectionString());
            });
        });

        builder.UseSetting("ConnectionStrings:DefaultConnection", _mssqlContainer.GetConnectionString());
        builder.UseSetting("ConnectionStrings:Redis", _redisContainer.GetConnectionString());
    }
}