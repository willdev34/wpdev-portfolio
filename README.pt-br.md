# WPDev Portfolio

**[🇺🇸 Read in English](./README.md)**

Um portfólio profissional para Desenvolvedor Full Stack Sênior, com identidade visual editorial e minimalista. Projetos em destaque, mini-blog em estilo revista, timeline interativa e área administrativa completa, tudo com uma API REST construída em Clean Architecture.

**Site:** [wpdev-portfolio-web.onrender.com](https://wpdev-portfolio-web.onrender.com)
**API:** [wpdev-portfolio-api.onrender.com](https://wpdev-portfolio-api.onrender.com) · [Swagger](https://wpdev-portfolio-api.onrender.com/swagger)

---

## Stack

| Camada | Tecnologia |
|---|---|
| API | .NET 8, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Banco de dados | PostgreSQL (Docker local, Supabase em produção) |
| Arquitetura | Clean Architecture, CQRS via MediatR |
| Validação / Mapeamento | FluentValidation, AutoMapper |
| Autenticação | ASP.NET Core Identity + JWT |
| Mídia | Cloudinary |
| Logging | Serilog |
| Testes | xUnit, Moq, coverlet |
| CI/CD | GitHub Actions |
| Hospedagem | Render (API + Web), Supabase (banco) |

## Arquitetura

A solution segue Clean Architecture em quatro camadas, mais um worker de background:

```
src/
├── Portfolio.Domain          # Entidades, sem dependências externas
├── Portfolio.Application     # CQRS (commands/queries via MediatR), DTOs, validators
├── Portfolio.Infrastructure  # EF Core, repositórios, serviços externos (Cloudinary, etc.)
├── Portfolio.Api             # API REST, controllers, autenticação
├── Portfolio.Web             # Frontend Blazor WebAssembly
└── Portfolio.Worker          # Serviço de background (criado, ainda sem funcionalidade ligada a ele)

tests/
├── Portfolio.UnitTests        # Testes das camadas Application e Domain
└── Portfolio.IntegrationTests # WebApplicationFactory + Testcontainers (em andamento)
```

As dependências apontam pra dentro: `Api` e `Web` dependem de `Application` e `Infrastructure`; `Application` depende só de `Domain`. `Infrastructure` implementa as interfaces definidas em `Application`.

## Funcionalidades

**Site público**
- Página inicial editorial com hero forte
- Listagem de projetos com thumbnails amplas e página de detalhe por projeto
- Timeline interativa
- Mini-blog em estilo revista digital
- Seção "O que estou fazendo agora"
- Galeria de fotos
- Formulário de contato minimalista

**Área administrativa**
- Área autenticada via JWT com CRUD completo de Projetos, Blog, Timeline, seção Now e mensagens de contato
- Upload de imagens via Cloudinary
- Layout responsivo com menu off-canvas animado em tablet e mobile

## API

8 controllers, com endpoints REST para autenticação, projetos, posts do blog, eventos da timeline, seção now, imagens da galeria, mensagens de contato e health check. A documentação completa dos endpoints está disponível via Swagger em `/swagger` ao rodar a API.

## Como rodar localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/) (para PostgreSQL, pgAdmin, Redis e MailDev locais)

### 1. Clonar e subir a infraestrutura local

```bash
git clone https://github.com/willdev34/wpdev-portfolio.git
cd wpdev-portfolio
docker-compose up -d
```

Isso sobe:

| Serviço | Porta | Finalidade |
|---|---|---|
| PostgreSQL | `5432` | Banco de dados da aplicação |
| pgAdmin | `5050` | Interface de administração do banco |
| Redis | `6379` | Provisionado para cache futuro, ainda não usado pela aplicação |
| MailDev | `1025` (SMTP) / `1080` (interface) | Captura emails enviados localmente, para testes |

### 2. Configurar segredos locais

A API precisa de um arquivo `src/Portfolio.Api/appsettings.Development.json`, que é ignorado pelo Git e nunca é commitado. Crie ele com seus próprios valores locais:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=portfolio_dev;Username=wpdev;Password=Dev2024"
  },
  "Jwt": {
    "Secret": "SUBSTITUA_POR_UM_SECRET_LOCAL_ALEATORIO_COM_32_CARACTERES_OU_MAIS"
  },
  "Admin": {
    "Email": "admin@wpdev.com",
    "Password": "SUBSTITUA_POR_UMA_SENHA_LOCAL"
  }
}
```

A aplicação lança uma exceção na inicialização se qualquer um desses valores estiver ausente. Isso é proposital: um ambiente mal configurado falha rápido, em vez de cair silenciosamente num valor padrão fraco.

### 3. Rodar a API e o frontend

```bash
dotnet run --project src/Portfolio.Api    # http://localhost:5277 (Swagger abre automaticamente)
dotnet run --project src/Portfolio.Web    # http://localhost:5237
```

## Testes

```bash
dotnet test
```

Os testes unitários cobrem as camadas Application e Domain, com xUnit e Moq. A cobertura é medida com coverlet e é obrigatória no CI: pull requests são bloqueados se a cobertura de linhas/statements ficar abaixo de 80%. Os testes de integração (`Portfolio.IntegrationTests`) usam `WebApplicationFactory` e Testcontainers para PostgreSQL, e estão em expansão ativa.

## CI/CD

O GitHub Actions roda em todo push e pull request:

1. **build-backend** — restaura, compila e roda os testes unitários da solution .NET
2. **quality-gate** — aplica o limite mínimo de 80% de cobertura, bloqueando o PR se não for atingido
3. **build-frontend** — compila o frontend Blazor WebAssembly

A branch `main` é protegida: só aceita mudanças via pull request que passem em todas as checagens. O deploy no Render é disparado automaticamente a cada merge na `main`.

## Monitoramento

- `GET /api/health` verifica a conectividade com o banco
- [UptimeRobot](https://uptimerobot.com/) faz ping na API e no frontend a cada 5 minutos
- Um job agendado no GitHub Actions mantém o banco Supabase ativo no plano gratuito

## Segurança

Segredos (chave de assinatura JWT, string de conexão do banco, credenciais de admin) são fornecidos via variáveis de ambiente em produção, e via um `appsettings.Development.json` ignorado pelo Git localmente. Nenhum valor de segredo é commitado neste repositório.

## Licença

© 2026 Will — WPDev. Todos os direitos reservados. Este é um projeto de portfólio pessoal; o código-fonte é público para fins de demonstração e não está licenciado para reuso.

## Contato

Desenvolvido por Will, WPDev — Desenvolvedor Full Stack Sênior no Brasil, atendendo clientes no Rio de Janeiro, Fortaleza e internacionalmente.