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

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MinitwitDbContext>(options =>
    options.UseSqlite(connection));

// Configure Serilog
var logFolder = builder.Configuration["LogLocation:LogFolder"];
var  outputTemplate = new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3} {Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}");
var logger = new LoggerConfiguration()
    // Log to console
    .WriteTo.Console(outputTemplate)
    .Enrich.FromLogContext()
    //log everything to a file
    .WriteTo.File(outputTemplate,$"{logFolder}/all/all/api_log-.txt", rollingInterval: RollingInterval.Day)
    //log errors to a separate file as well
    .WriteTo.File(outputTemplate, $"{logFolder}/all/errors/api_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)  
    //filter to log things from the follow related code
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("FollowerController") ||
                                      e.Properties["SourceContext"].ToString().Contains("FollowService") ||
                                      e.Properties["SourceContext"].ToString().Contains("FollowRepository")))
        .WriteTo.File(outputTemplate, $"{logFolder}/follow/all/follow_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File(outputTemplate, $"{logFolder}/follow/errors/follow_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    //filter to log things from the message related code
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("MessageController") ||
                                      e.Properties["SourceContext"].ToString().Contains("MessageService") ||
                                      e.Properties["SourceContext"].ToString().Contains("MessageRepository")))
        .WriteTo.File(outputTemplate, $"{logFolder}/message/all/message_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File(outputTemplate, $"{logFolder}/message/errors/message_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    //filter to log things from the user related code
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("RegisterController") ||
                                      e.Properties["SourceContext"].ToString().Contains("UserService") ||
                                      e.Properties["SourceContext"].ToString().Contains("UserRepository")))
        .WriteTo.File(outputTemplate, $"{logFolder}/user/all/user_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File(outputTemplate, $"{logFolder}/user/errors/user_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    //filter to log latest from the user related code
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("LatestRepository")))
        .WriteTo.File(outputTemplate, $"{logFolder}/latest/all/user_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File(outputTemplate, $"{logFolder}/latest/errors/user_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    .CreateLogger();

// Add Serilog to .NET logging system
Log.Logger = logger;
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.MapPrometheusScrapingEndpoint();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace Api
{
    public partial class Program
    {
    }
}