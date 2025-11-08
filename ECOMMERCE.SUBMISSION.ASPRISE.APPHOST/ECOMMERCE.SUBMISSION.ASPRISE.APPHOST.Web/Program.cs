using Blazored.LocalStorage;
using ECOMMERCE.SUBMISSION.WEB.ADMIN;
using ECOMMERCE.SUBMISSION.WEB.ADMIN.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();
builder.AddAuthentication();
builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new("http://apiservice"));
builder.Services.AddHttpClient<AccountApiClient>(client => client.BaseAddress = new("http://ecommerce-submission-api-account"));
builder.Services.AddHttpClient<OrderApiClient>(client => client.BaseAddress = new("http://ecommerce-submission-api-order"));
builder.Services.AddHttpClient<PaymentApiClient>(client => client.BaseAddress = new("http://ecommerce-submission-api-payment"));
// Local storage save JWT
builder.Services.AddBlazoredLocalStorage();

// Custom Auth Provider
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddAuthenticationCore();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
