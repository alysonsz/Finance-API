using Finance.Application.Handlers;
using Finance.Application.Interfaces.Handlers;
using Finance.Application.Interfaces.Repositories;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Finance.Api.Extensions;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebApplicationBuilder builder)
    {
        ApiConfiguration.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }

    public static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<FinanceDbContext>(o =>
            o.UseSqlServer(ApiConfiguration.ConnectionString));
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
        options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                    .WithOrigins("https://localhost:7243")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            ));
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x => x.CustomSchemaIds(n => n.FullName));
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
        builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

        builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
        builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
    }
}