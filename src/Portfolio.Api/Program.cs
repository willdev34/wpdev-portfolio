// ====================================
// Título: Program.cs - API Entry Point
// Descrição: Ponto de entrada da API, configura serviços e middleware
// Autor: Will
// Empresa: WpDev
// Data: 23/11/2024
// ====================================

using Microsoft.EntityFrameworkCore;
using Portfolio.Infrastructure.Data;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ====================================
// Registra o DbContext e configura a conexão com PostgreSQL
// A connection string vem do appsettings.json
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====================================
// CONFIGURAÇÃO DO AUTOMAPPER
// ====================================
// Registra todos os Profiles da camada Application
// Isso permite converter automaticamente Entity ↔ DTO
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<Portfolio.Application.Mappings.ProjectMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.BlogPostMappingProfile>();
});

// ====================================
// CONFIGURAÇÃO DO AUTOMAPPER
// ====================================
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<Portfolio.Application.Mappings.ProjectMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.BlogPostMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.TimelineEventMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.GalleryImageMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.ContactMessageMappingProfile>();
    config.AddProfile<Portfolio.Application.Mappings.NowSectionMappingProfile>();
});

// ====================================
// REGISTRO DOS REPOSITORIES
// ====================================
// Registra o ProjectRepository para injeção de dependência
// Sempre que alguém pedir IProjectRepository, o ASP.NET injeta ProjectRepository
builder.Services.AddScoped<Portfolio.Application.Interfaces.IProjectRepository, 
                           Portfolio.Infrastructure.Repositories.ProjectRepository>();

// Registra o BlogPostRepository para injeção de dependência
builder.Services.AddScoped<Portfolio.Application.Interfaces.IBlogPostRepository, 
                           Portfolio.Infrastructure.Repositories.BlogPostRepository>();  

// Registra o TimelineEventRepository para injeção de dependência
builder.Services.AddScoped<Portfolio.Application.Interfaces.ITimelineEventRepository, 
                           Portfolio.Infrastructure.Repositories.TimelineEventRepository>();   

builder.Services.AddScoped<Portfolio.Application.Interfaces.IGalleryImageRepository, 
                           Portfolio.Infrastructure.Repositories.GalleryImageRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.IContactMessageRepository, 
                           Portfolio.Infrastructure.Repositories.ContactMessageRepository>();  

builder.Services.AddScoped<Portfolio.Application.Interfaces.INowSectionRepository, 
                           Portfolio.Infrastructure.Repositories.NowSectionRepository>();                                                         

// ====================================
// CONFIGURAÇÃO DO MEDIATR (CQRS)
// ====================================
// Registra todos os Handlers (Commands e Queries) da camada Application
// O MediatR vai procurar automaticamente todos os IRequestHandler
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Portfolio.Application.Queries.Projects.GetAllProjects.GetAllProjectsQuery).Assembly));

// ====================================
// CONFIGURAÇÃO DO FLUENTVALIDATION
// ====================================
// Registra todos os Validators da camada Application
// As validações serão executadas automaticamente antes dos Handlers
builder.Services.AddValidatorsFromAssembly(typeof(Portfolio.Application.Commands.Projects.CreateProject.CreateProjectCommandValidator).Assembly);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// ====================================
// CONFIGURAÇÃO DOS CONTROLLERS
// ====================================
// Habilita o uso de Controllers (API REST tradicional)
builder.Services.AddControllers();

// ====================================
// CONFIGURAÇÃO DO CORS
// ====================================
// Permite que o Blazor (localhost:5237) acesse a API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("http://localhost:5237", "https://localhost:7257")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ====================================
// APLICAR MIGRATIONS AUTOMATICAMENTE
// ====================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ====================================
// USA O CORS
// ====================================
app.UseCors("AllowBlazor");

// ====================================
// MAPEAMENTO DOS CONTROLLERS
// ====================================
// Registra todos os Controllers da API
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
