# WPDev Portfolio

**[🇺🇸 Read in English](./README.md)**

Um portfólio profissional para Desenvolvedor Full Stack Sênior, com identidade visual editorial e minimalista. Projetos em destaque, mini-blog em estilo revista, timeline interativa e área administrativa completa, tudo com uma API REST construída em Clean Architecture.

**Site:** [www.wpdevbr.com](https://www.wpdevbr.com)
**API:** mesmo domínio · [Swagger](https://www.wpdevbr.com/swagger)

---

## Stack

| Camada | Tecnologia |
|---|---|
| API | .NET 8, ASP.NET Core Web API |
| Frontend | Blazor WebAssembly |
| Banco de dados | PostgreSQL (Docker local, [Neon](https://neon.tech) em produção) |
| Arquitetura | Clean Architecture, CQRS via MediatR |
| Validação / Mapeamento | FluentValidation, AutoMapper |
| Autenticação | ASP.NET Core Identity + JWT |
| Mídia | Cloudinary |
| Logging | Serilog |
| Testes | xUnit, Moq, coverlet |
| CI/CD | GitHub Actions |
| Hospedagem | Vercel (API e Web, projeto único, runtime de container) |

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
└── Portfolio.IntegrationTests # WebApplicationFactory + Testcontainers
```

As dependências apontam pra dentro: `Api` e `Web` dependem de `Application` e `Infrastructure`; `Application` depende só de `Domain`. `Infrastructure` implementa as interfaces definidas em `Application`.

## Infraestrutura

A API e o frontend rodam como dois containers separados ([Vercel Services](https://vercel.com/docs/services)) dentro de um único projeto Vercel, compartilhando o mesmo domínio, roteados por path:

- `/api/*` e `/swagger*` → o container `api` (.NET 8, Kestrel)
- todo o resto → o container `web` (nginx servindo o build do Blazor WebAssembly)

Como os dois serviços dividem a mesma origem, o frontend chama a API no mesmo domínio, sem precisar de CORS em produção. O `HttpClient` do frontend usa `HostEnvironment.BaseAddress` exatamente por isso: funciona sem mudança nenhuma no domínio de produção, nas URLs de preview geradas automaticamente pelo Vercel, e em qualquer domínio futuro, sem configuração.

**Ambientes**

| Ambiente | Gatilho | Banco de dados |
|---|---|---|
| Produção | Merge na `main` | Branch `production` do Neon |
| Preview | Push na `develop` | Branch `develop` do Neon (cópia copy-on-write da `production`) |
| Local | `dotnet run` | PostgreSQL via Docker Compose |

As migrations não rodam mais automaticamente a cada subida do container. Antes rodavam, e isso adicionava vários segundos na primeira requisição depois do Vercel escalar o container a zero; agora são aplicadas deliberadamente, com `dotnet ef database update` direto contra o Neon (conexão direta, sem pooler).

Migrado de Render + Supabase para Vercel + Neon em setembro de 2026.

## Funcionalidades

**Site público**
- Página inicial editorial com hero forte
- Listagem de projetos com thumbnails amplas, selo de ano/status em cada card, e página de detalhe por projeto
- Timeline interativa
- Mini-blog em estilo revista digital
- Seção "O que estou fazendo agora"
- Galeria de fotos
- Formulário de contato minimalista

**Área administrativa**
- Área autenticada via JWT com CRUD completo de Projetos, Blog, Timeline, seção Now e mensagens de contato
- Dashboard com card visual por seção, imagem de fundo por assunto
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

Os testes unitários cobrem as camadas Application e Domain, com xUnit e Moq. A cobertura é medida com coverlet e é obrigatória no CI: pull requests são bloqueados se a cobertura de métodos ficar abaixo de 80%. Os testes de integração (`Portfolio.IntegrationTests`) usam `WebApplicationFactory` e Testcontainers para PostgreSQL.

## CI/CD

O GitHub Actions roda em todo push e pull request:

1. **build-backend** — restaura, compila e roda os testes unitários da solution .NET
2. **quality-gate** — aplica o limite mínimo de 80% de cobertura, bloqueando o PR se não for atingido
3. **build-frontend** — compila o frontend Blazor WebAssembly

A branch `main` é protegida: só aceita mudanças via pull request que passem em todas as checagens. O deploy no Vercel é disparado automaticamente a cada push: na `main` pra produção, na `develop` pra um ambiente de preview com sua própria branch de banco.

## Monitoramento

- `GET /api/health` verifica a conectividade com o banco
- Os dois containers escalam a zero depois de um período de inatividade, por design, não é uma limitação a contornar. Um job de `keep-alive` no GitHub Actions, necessário nos planos gratuitos do Render/Supabase, foi removido quando a infraestrutura migrou pra Vercel + Neon.

## 🔒 Segurança

Segredos (chave de assinatura JWT, string de conexão do banco, credenciais de admin) são fornecidos via variáveis de ambiente em produção, e via um `appsettings.Development.json` ignorado pelo Git localmente. Nenhum valor de segredo é commitado neste repositório.

### Nota histórica: Row Level Security no Supabase

O banco de produção rodou no Supabase até setembro de 2026. Todas as tabelas do schema `public` tinham RLS habilitado, incluindo as tabelas de negócio (`Projects`, `BlogPosts`, `ContactMessages`, `GalleryImages`, `NowSections`, `TimelineEvents`) e as tabelas do ASP.NET Identity (`AspNetUsers`, `AspNetRoles`, etc), porque o Supabase expõe automaticamente uma API REST (PostgREST) para qualquer tabela pública, acessível via `anon key`. Sem RLS, essa API permitia leitura e escrita direta, ignorando completamente a autenticação JWT da aplicação. A aplicação conectava usando o role padrão do Supabase, que tem privilégio de bypass de RLS, por isso o RLS foi habilitado em todas as tabelas sem necessidade de criar policies: o backend continuava funcionando normalmente, e a API REST pública do Supabase passava a negar acesso por padrão.

Essa mitigação não se aplica ao banco de produção atual (Neon), que não expõe uma superfície pública equivalente. Mantido aqui como registro de uma descoberta e correção reais.

Referência: [Supabase Advisor](https://supabase.com/docs/guides/database/database-advisors)

## Licença

© 2026 Will — WPDev. Todos os direitos reservados. Este é um projeto de portfólio pessoal; o código-fonte é público para fins de demonstração e não está licenciado para reuso.

## Contato

Desenvolvido por Will, WPDev — Desenvolvedor Full Stack Sênior, atendendo clientes a nível Brasil.