using Finance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
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

        try
        {
            var context = services.GetRequiredService<FinanceDbContext>();

            if (context.Database.IsRelational() && context.Database.GetPendingMigrations().Any())
            {
                Log.Information("Applying pending migrations...");
                await context.Database.MigrateAsync();
                Log.Information("Database updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while applying database migrations.");
            throw;
        }
    }
}
