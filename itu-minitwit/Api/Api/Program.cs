using Api;
using Api.DataAccess;
using Api.DataAccess.Models;
using Api.DataAccess.Repositories;
using Api.Services.RepositoryInterfaces;
using Api.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Templates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// OpenTelemetry for exposing metrics for Prometheus
builder.Services.AddSingleton<MetricsConfig>();

var resourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(MetricsConfig.ServiceName, serviceVersion: MetricsConfig.ServiceVersion);

builder.Services.AddOpenTelemetry().WithMetrics(metrics =>
{
    metrics.SetResourceBuilder(resourceBuilder)
        .AddAspNetCoreInstrumentation() // Enables HTTP metrics
        .AddHttpClientInstrumentation() // Enables outgoing request metrics
        .AddMeter(
            "Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel",
            MetricsConfig.ServiceName
        )
        .AddPrometheusExporter();
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

//Repositories
builder.Services.AddScoped<ILatestRepository, LatestRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

//Services
builder.Services.AddScoped<ILatestService, LatestService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Testing")
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                           ?? throw new ArgumentException("Cant find connection string");

    builder.Services.AddDbContext<MinitwitDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// Configure logging
// builder.Host.UseSerilog((context, loggerConfig) =>
//     loggerConfig.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//apply pending migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MinitwitDbContext>();
    context.Database.Migrate();
    if (!context.LatestProcessedSimActions.Any())
    {
        context.LatestProcessedSimActions.Add(new LatestProcessedSimAction { Latest = -1 });
        context.SaveChanges();
    }
}
// app.UseSerilogRequestLogging();

app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace Api
{
    public partial class Program
    {
    }
}