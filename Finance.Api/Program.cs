using Finance.Api;
using Finance.Api.Extensions;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando a aplicação Finance API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());
    builder.AddConfiguration();
    builder.AddDatabase();
    builder.AddCors();
    builder.AddDocumentation();
    builder.AddServices();

    builder.Services.AddMediatR(typeof(Finance.Application.AssemblyReference).Assembly);
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddControllers();
    builder.Services.AddJwtAuthentication(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors(ApiConfiguration.CorsPolicyName);

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<Finance.Infrastructure.Data.FinanceDbContext>();

            if (context.Database.IsRelational() && (context.Database.GetPendingMigrations().Any()))
            {
                Log.Information("Aplicando Migrations no Banco de Dados...");
                context.Database.Migrate();
                Log.Information("Banco de Dados atualizado com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Ocorreu um erro ao aplicar as Migrations.");
            throw;
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação encerrou inesperadamente durante a inicialização (Startup Failure).");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }