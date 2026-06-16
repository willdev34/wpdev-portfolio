// ====================================
// Título: Program.cs - Blazor WASM Entry Point
// Descrição: Configuração inicial do Blazor WebAssembly com autenticação
// ====================================

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portfolio.Web;
using Portfolio.Web.Auth;
using Portfolio.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ====================================
// CONFIGURAÇÃO DO HTTPCLIENT
// ====================================
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5277/";
Console.WriteLine($"[Program] API URL: {apiBaseUrl}");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// ====================================
// CONFIGURAÇÃO DE AUTENTICAÇÃO
// ====================================
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<JwtAuthStateProvider>());


// ====================================
// REGISTRO DOS SERVIÇOS
// ====================================
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<BlogPostService>();
builder.Services.AddScoped<TimelineService>();
builder.Services.AddScoped<GalleryService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<NowService>();
builder.Services.AddScoped<ContactMessageService>();


await builder.Build().RunAsync();
