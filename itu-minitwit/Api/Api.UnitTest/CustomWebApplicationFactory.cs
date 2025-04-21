using System.Data.Common;
using Api.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace Api.UnitTest;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlTestContainer postgresContainer;

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        postgresContainer = new PostgreSqlTestContainer();
        postgresContainer.InitializeAsync().Wait();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<MinitwitDbContext>));
            if (dbContextDescriptor != null) services.Remove(dbContextDescriptor);

            services.AddControllersWithViews();
            
            services.AddDbContext<MinitwitDbContext>(options =>
            {
                options.UseNpgsql(postgresContainer.ConnectionString);
                options.EnableSensitiveDataLogging();
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await postgresContainer.ResetAsync();
    }
    
    public MinitwitDbContext GetDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<MinitwitDbContext>();
    }

    public new async Task DisposeAsync()
    {
        await postgresContainer.DisposeAsync();
    }
}