using System.Reflection;
using Frontend.Blazor.Code;
using Frontend.Blazor.Data;
using Frontend.Blazor.HttpClients;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddTransient<LoginService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddHttpClient<IBackendApiHttpClient, BackendApiHttpClient>(options =>
{
    options.BaseAddress = new Uri(builder.Configuration.GetValue<string>("Urls:BackendApi"));
    options.Timeout = TimeSpan.FromSeconds(30);
    options.DefaultRequestHeaders.TryAddWithoutValidation("Service", Assembly.GetAssembly(typeof(Program))?.GetName().Name);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();