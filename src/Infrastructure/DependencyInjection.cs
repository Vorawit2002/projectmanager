using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ProjectManagement.Application;
using ProjectManagement.Application.Common.Interfaces;
using ProjectManagement.Domain.Constants;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure;
using ProjectManagement.Infrastructure.Data;
using ProjectManagement.Infrastructure.Data.Interceptors;
using ProjectManagement.Infrastructure.FileStorage;
using ProjectManagement.Infrastructure.Identity;
using ProjectManagement.Infrastructure.Services;
using Serilog;
using Serilog.Core;
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? configuration.GetConnectionString("DefaultConnection");
        Console.Write(Environment.GetEnvironmentVariable("DefaultConnection"));
        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        var levelSwitch = new LoggingLevelSwitch();
        Serilog.Log.Logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(levelSwitch)
            .WriteTo.Seq(Environment.GetEnvironmentVariable("SeqServerUrl") ?? configuration["Seq:SeqServerUrl"]??string.Empty, controlLevelSwitch: levelSwitch)
            .CreateLogger();

        Serilog.Log.Information("Starting up");

        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.AddSeq(Environment.GetEnvironmentVariable("SeqServerUrl") ?? configuration["Seq:SeqServerUrl"]);
        });

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        builder.Services.AddScoped<IMinIOService, MinIOService>();
        builder.Services.AddTransient<IStoragePathService, StoragePathService>();
        builder.Services.AddScoped<PushNotificationService>();
        builder.Services.AddHostedService<DailyPushNotificationWorker>();
        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });


        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<IEmailSenderService, EmailSenderService>(); //Email
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();
        builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>(); //SoftDelete
        builder.Services.AddScoped<SmartCardService>();

        // JWT Authentication Configuration
        var jwtSecretKey = Environment.GetEnvironmentVariable("JwtSecretKey") ?? configuration["JwtSettings:SecretKey"];
        var jwtIssuer = Environment.GetEnvironmentVariable("JwtIssuer") ?? configuration["JwtSettings:Issuer"];
        var jwtAudience = Environment.GetEnvironmentVariable("JwtAudience") ?? configuration["JwtSettings:Audience"];

        Guard.Against.NullOrEmpty(jwtSecretKey, message: "JWT SecretKey is not configured");
        Guard.Against.NullOrEmpty(jwtIssuer, message: "JWT Issuer is not configured");
        Guard.Against.NullOrEmpty(jwtAudience, message: "JWT Audience is not configured");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("❌ JWT auth failed: " + context.Exception?.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });
        //HangFire
        //======================================
        builder.Services.AddScoped<IHangfireEmailSender, HangfireEmailSender>();
        var HangfireConnectionString = Environment.GetEnvironmentVariable("HangfireConnection") ?? configuration.GetConnectionString("HangfireConnection");
        Guard.Against.Null(HangfireConnectionString, message: "Connection string 'HangfireConnectionString' not found.");

        // Add Hangfire services
        builder.Services.AddHangfire(configuration =>
        {
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            configuration.UseSimpleAssemblyNameTypeSerializer();
            configuration.UseRecommendedSerializerSettings();
            configuration.UsePostgreSqlStorage(c =>
            {
                c.UseNpgsqlConnection(HangfireConnectionString); // 
            });
        }).AddHangfireServer(option => {
            option.WorkerCount = 2; //จำนวนครั้งการ Resend Email
        });
        

        //========================================
        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

        builder.Services.AddAuthorization(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
    }
}
