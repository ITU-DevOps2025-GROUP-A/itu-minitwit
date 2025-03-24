using Web.Components;
using Web.DataAccess;
using Web.Services;
using Web.Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment-specific appsettings.json, into the program configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<UserState>();

//repository's
builder.Services.AddHttpClient<IMessageRepository, MessageRepository>();
builder.Services.AddHttpClient<IUserRepository, UserRepository>();
builder.Services.AddHttpClient<IFollowRepository, FollowRepository>();

//services
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
