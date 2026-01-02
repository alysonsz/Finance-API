using Finance.Api;
using Finance.Api.Extensions;
using FluentValidation.AspNetCore;
using System.Reflection;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Finance.Application.AssemblyReference).Assembly));
builder.AddConfiguration();
builder.AddDatabase();
builder.AddCors();
builder.AddDocumentation();
builder.AddServices();

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

app.Run();

public partial class Program { }