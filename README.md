# 🎴 WPDev Portfolio

> Portfólio profissional com cards estilo Yu-Gi-Oh!, desenvolvido com .NET 8, Blazor WebAssembly e Clean Architecture.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=flat&logo=blazor)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?style=flat&logo=postgresql)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=flat&logo=docker)

## 🌟 Visão Geral

Site de portfólio pessoal visualmente marcante que combina:
- **Cards de Projetos** estilo cartas de jogo com animações flip 3D
- **Timeline Interativa** de eventos pessoais e profissionais
- **Mini-Blog** com posts em Markdown
- **Galeria de Imagens** com lightbox
- **Área Admin** completa para gerenciar conteúdo
- **Formulário de Contato** com proteção anti-spam

## 🏗️ Arquitetura

Projeto seguindo **Clean Architecture** com separação em camadas:
- **Domain**: Entidades e interfaces (sem dependências)
- **Application**: Casos de uso, DTOs, CQRS (MediatR)
- **Infrastructure**: EF Core, repositórios, serviços externos
- **API**: Controllers, autenticação JWT
- **Web**: Blazor WebAssembly UI

## 🚀 Stack Tecnológica

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core** - ORM
- **MediatR** - Padrão CQRS
- **JWT Bearer** - Autenticação

### Frontend
- **Blazor WebAssembly** - SPA framework
- **Tailwind CSS** - Estilização (manual da marca WPDev)

### Infraestrutura
- **PostgreSQL 15** - Banco de dados
- **Redis** - Cache
- **Docker Compose** - Orquestração
- **MailDev** - Testes de email (dev)

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/)
- [Git](https://git-scm.com/)

## 🎯 Como Executar

### 1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/wpdev-portfolio.git
cd wpdev-portfolio
```

### 2. Suba os containers Docker
```bash
docker-compose up -d
```

### 3. Restaure as dependências
```bash
dotnet restore
```

### 4. Execute o build
```bash
dotnet build
```

### 5. Execute a aplicação
```bash
# API
cd src/Portfolio.Api
dotnet run

# Blazor (outro terminal)
cd src/Portfolio.Web
dotnet run
```

## 🔑 Credenciais de Desenvolvimento

### PostgreSQL
- **Host**: localhost:5432
- **Database**: portfolio_dev
- **User**: wpdev
- **Password**: Dev@2024!

### pgAdmin
- **URL**: http://localhost:5050
- **Email**: admin@wpdev.com
- **Password**: Admin@2024!

### MailDev
- **URL**: http://localhost:1080

## 📁 Estrutura do Projeto
```
wpdev-portfolio/
├── src/
│   ├── Portfolio.Domain/          # Entidades, interfaces
│   ├── Portfolio.Application/     # Casos de uso, CQRS
│   ├── Portfolio.Infrastructure/  # EF Core, repositórios
│   ├── Portfolio.Api/             # Web API
│   ├── Portfolio.Web/             # Blazor WASM
│   └── Portfolio.Worker/          # Background jobs
├── tests/
│   ├── Portfolio.UnitTests/
│   └── Portfolio.IntegrationTests/
├── docs/                          # Documentação
├── deployments/
│   └── docker/                    # Dockerfiles
├── docker-compose.yml
└── Portfolio.sln
```

## 🎨 Design - Manual da Marca WPDev

### Cores Principais
- **WPDev Azul-Primário**: `#6C9EA3`
- **WPDev Dark**: `#0D1C24`
- **Cinza Neutro Claro**: `#F4F7F7`
- **Cinza Médio**: `#A3A9AB`

### Tipografia
- **Primária**: Inter (UI e textos)
- **Secundária**: Poppins (destaques e títulos)

## 🗺️ Roadmap

- [x] Sprint 0: Configuração inicial e infraestrutura
- [ ] Sprint 1: Domain entities e database
- [ ] Sprint 2: API endpoints básicos
- [ ] Sprint 3: Blazor UI - Cards e Grid
- [ ] Sprint 4: Autenticação e área Admin
- [ ] Sprint 5: Blog e Timeline
- [ ] Sprint 6: Gallery e Contact Form
- [ ] Sprint 7: Performance e SEO
- [ ] Sprint 8: Deploy e documentação final

## 📄 Licença

Este projeto está sob a licença MIT.

## 📞 Contato

**William (Willzin)**
- GitHub: [@seu-usuario](https://github.com/willdev34)
- LinkedIn: [seu-perfil](https://www.linkedin.com/in/willdevfull/)

---

**Desenvolvido com 💙 usando .NET 8 e Clean Architecture**