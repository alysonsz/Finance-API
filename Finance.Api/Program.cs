using Finance.Api;
using Finance.Api.Extensions;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
builder.AddDatabase();
builder.AddCors();
builder.AddDocumentation();
builder.AddServices();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapControllers();

app.Run();