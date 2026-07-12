// ====================================
// Título: Program.cs - API Entry Point
// Descrição: Ponto de entrada da API, configura serviços e middleware
// ====================================

using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Application.Interfaces;
using Portfolio.Infrastructure.Data;
using Portfolio.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ====================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ====================================
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====================================
// CONFIGURAÇÃO DO IDENTITY
// ====================================
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    // Requisitos de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // Lockout após 5 tentativas erradas
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Email único
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PortfolioDbContext>()
.AddDefaultTokenProviders();

// ====================================
// CONFIGURAÇÃO DO JWT
// ====================================
var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret não configurado.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<Portfolio.Application.Interfaces.IProjectRepository,
                           Portfolio.Infrastructure.Repositories.ProjectRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.IBlogPostRepository,
                           Portfolio.Infrastructure.Repositories.BlogPostRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.ITimelineEventRepository,
                           Portfolio.Infrastructure.Repositories.TimelineEventRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.IGalleryImageRepository,
                           Portfolio.Infrastructure.Repositories.GalleryImageRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.IContactMessageRepository,
                           Portfolio.Infrastructure.Repositories.ContactMessageRepository>();

builder.Services.AddScoped<Portfolio.Application.Interfaces.INowSectionRepository,
                           Portfolio.Infrastructure.Repositories.NowSectionRepository>();

// ====================================
// REGISTRO DO JWT SERVICE
// ====================================
builder.Services.AddScoped<IJwtService, JwtService>();

// ====================================
// REGISTRO DO EMAIL SERVICE
// ====================================
builder.Services.AddScoped<Portfolio.Application.Interfaces.IEmailService,
                           Portfolio.Infrastructure.Services.EmailService>();

// ====================================
// CONFIGURAÇÃO DO MEDIATR (CQRS)
// ====================================
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(Portfolio.Application.Queries.Projects.GetAllProjects.GetAllProjectsQuery).Assembly));

// ====================================
// CONFIGURAÇÃO DO FLUENTVALIDATION
// ====================================
builder.Services.AddValidatorsFromAssembly(
    typeof(Portfolio.Application.Commands.Projects.CreateProject.CreateProjectCommandValidator).Assembly);

// ====================================
// CONFIGURAÇÃO DOS CONTROLLERS
// ====================================
builder.Services.AddControllers();

// ====================================
// CONFIGURAÇÃO DO CORS
// ====================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5237",
                "https://localhost:7257",
                "https://wpdev-portfolio-web.onrender.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Permite enviar token JWT pelo Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT. Exemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ====================================
// APLICAR MIGRATIONS E SEED ADMIN
// ====================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    db.Database.Migrate();

    // Seed do usuário admin
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var adminEmail = builder.Configuration["Admin:Email"]
        ?? throw new InvalidOperationException("Admin:Email não configurado.");
    var adminPassword = builder.Configuration["Admin:Password"]
        ?? throw new InvalidOperationException("Admin:Password não configurado.");

    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

    if (existingAdmin is null)
    {
        var admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Will - WpDev",
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, adminPassword);
    }
    else if (builder.Configuration.GetValue<bool>("Admin:ForceResetPassword"))
    {
        // Reset controlado da senha, ativado só via variável de ambiente
        // Admin__ForceResetPassword=true. Desativar depois de confirmar o login.
        var removeResult = await userManager.RemovePasswordAsync(existingAdmin);
        if (removeResult.Succeeded)
        {
            await userManager.AddPasswordAsync(existingAdmin, adminPassword);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");

// Ordem importa: Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();