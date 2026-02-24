using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;

namespace Finance.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors(ApiConfiguration.CorsPolicyName);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        for (int i = 1; i <= 6; i++)
        {
            try
            {
                var writeDb = services.GetRequiredService<FinanceWriteDbContext>();
                var databaseCreator = writeDb.GetService<IRelationalDatabaseCreator>();

                if (!await databaseCreator.ExistsAsync())
                {
                    Log.Information("Database does not exist. Creating...");
                    await databaseCreator.CreateAsync();
                }

                await writeDb.Database.MigrateAsync();

                var readDb = services.GetRequiredService<FinanceReadDbContext>();
                var readCreator = readDb.GetService<IRelationalDatabaseCreator>();

                if (!await readCreator.ExistsAsync())
                {
                    Log.Information("READ Database (Postgres) does not exist. Creating...");
                    await readCreator.CreateAsync();
                }

                await readDb.Database.MigrateAsync();

                Log.Information("Migrations applied successfully!");
                return;
            }
            catch (Exception ex)
            {
                if (i == 6) Log.Fatal(ex, "Final attempt failed.");
                Log.Warning("Attempt {i}: Database not ready. Retrying in 5s...", i);
                await Task.Delay(5000);
            }
        }
    }
}
