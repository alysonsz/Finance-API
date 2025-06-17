using Finance.Api;
using Finance.Api.Extensions;

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
app.UseAuthorization();
app.MapControllers();

app.Run();