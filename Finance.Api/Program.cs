using Finance.Api.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Finance API...");

    var builder = WebApplication.CreateBuilder(args);

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.AddApiInfrastructure();

    var app = builder.Build();

    app.UseApiPipeline();
    await app.ApplyDatabaseMigrationsAsync();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly during startup.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }