// ====================================
// Título: Program.cs - Blazor WASM Entry Point
// Descrição: Configuração inicial do Blazor WebAssembly
// Autor: Will
// Empresa: WpDev
// Data: 07/12/2024
// ====================================

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Portfolio.Web;
using Portfolio.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// ====================================
// CONFIGURAÇÃO DOS COMPONENTES RAIZ
// ====================================
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ====================================
// CONFIGURAÇÃO DO HTTPCLIENT
// ====================================
// IMPORTANTE: Aponta para a API que está rodando em http://localhost:5277
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5277/") 
});

// ====================================
// REGISTRO DOS SERVIÇOS
// ====================================
// Registra o ProjectService para injeção de dependência
builder.Services.AddScoped<ProjectService>();

// TODO: Adicionar outros serviços conforme formos criando
// builder.Services.AddScoped<BlogPostService>();
// builder.Services.AddScoped<TimelineEventService>();
// builder.Services.AddScoped<GalleryImageService>();
// builder.Services.AddScoped<ContactMessageService>();
// builder.Services.AddScoped<NowSectionService>();

await builder.Build().RunAsync();