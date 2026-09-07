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
// Em Development (dotnet run local), usa a URL configurada em
// appsettings.json, porque API e front rodam em portas diferentes.
// Em qualquer ambiente publicado (Production ou Preview no Vercel),
// usa a propria origem de onde o app foi carregado: front e API
// sempre estao no mesmo dominio (Opcao B do deploy), entao nao
// precisa mais fixar API_BASE_URL nenhum, funciona pra qualquer
// URL de preview gerada aleatoriamente.
var configuredApiBaseUrl = builder.Configuration["ApiBaseUrl"];
var apiBaseUrl = builder.HostEnvironment.IsDevelopment() && !string.IsNullOrWhiteSpace(configuredApiBaseUrl)
    ? configuredApiBaseUrl
    : builder.HostEnvironment.BaseAddress;

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
