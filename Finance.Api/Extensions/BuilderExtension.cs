using Finance.Api.Validators.Category;
using Finance.Application.Handlers;
using Finance.Contracts.Interfaces.Handlers;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using Finance.Infrastructure.Services;
using Finance.Web.Handlers;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        return services;
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(n => n.FullName);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                              "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: \"Bearer abcdef12345\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
        });
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
        builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
        builder.Services.AddTransient<IUserRepository, UserRepository>();

        builder.Services.AddTransient<ITokenService, TokenService>();

        builder.Services.AddTransient<ICategoryHandler, CategoryHandler>();
        builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
        builder.Services.AddTransient<IUserHandler, UserHandler>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();
    }
}