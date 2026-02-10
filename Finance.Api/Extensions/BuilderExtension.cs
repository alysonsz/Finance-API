using Finance.Api.Validators.Auth;
using Finance.Application;
using Finance.Application.Features.Auth.Register;
using Finance.Application.Features.Categories.Create;
using Finance.Application.Features.Categories.GetAll;
using Finance.Application.Features.Categories.Update;
using Finance.Application.Features.Transactions.Create;
using Finance.Application.Features.Transactions.GetByPeriod;
using Finance.Application.Features.Transactions.Update;
using Finance.Application.Services;
using Finance.Contracts.Interfaces.Repositories;
using Finance.Contracts.Interfaces.Services;
using Finance.Infrastructure.Data;
using Finance.Infrastructure.Repositories;
using Finance.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
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
            o.UseSqlServer(ApiConfiguration.ConnectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            }));
    }

    public static WebApplicationBuilder AddApiInfrastructure(this WebApplicationBuilder builder)
    {
        builder.AddConfiguration();
        builder.AddDatabase();
        builder.AddCache();
        builder.AddCors();
        builder.AddDocumentation();
        builder.AddServices();
        builder.AddMediatR();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddControllers();
        builder.Services.AddJwtAuthentication(builder.Configuration);

        return builder;
    }

    public static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(
        options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                    .SetIsOriginAllowed(origin => true)
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

    public static void AddCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            var connection = builder.Configuration.GetConnectionString("Redis");
            options.Configuration = connection;
            options.InstanceName = "Finance_";
        });
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

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(typeof(AssemblyReference).Assembly);
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ITransactionService, TransactionService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<ICacheService, CacheService>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateCategoryRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<GetAllCategoriesRequestValidator>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateTransactionRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateTransactionRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<GetByPeriodTransactionRequestValidator>();

        builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
    }
}