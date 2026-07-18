# ====================================
# Dockerfile - Portfolio.Web (Blazor WASM)
# Executado a partir da RAIZ do repositório
# ====================================

# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY src/Portfolio.Domain/Portfolio.Domain.csproj src/Portfolio.Domain/
COPY src/Portfolio.Application/Portfolio.Application.csproj src/Portfolio.Application/
COPY src/Portfolio.Web/Portfolio.Web.csproj src/Portfolio.Web/

# Restaurar dependências
RUN dotnet restore src/Portfolio.Web/Portfolio.Web.csproj

# Copiar código fonte
COPY src/ src/

# Build e publish
RUN dotnet publish src/Portfolio.Web/Portfolio.Web.csproj -c Release -o /app/publish

# Gerar sitemap.xml com os projetos e posts reais da API
RUN apt-get update && apt-get install -y --no-install-recommends curl jq && rm -rf /var/lib/apt/lists/*
COPY src/Portfolio.Web/generate-sitemap.sh /tmp/generate-sitemap.sh
RUN chmod +x /tmp/generate-sitemap.sh && /tmp/generate-sitemap.sh

# STAGE 2: Nginx para servir arquivos estáticos
FROM nginx:alpine AS runtime

# Copiar arquivos publicados do Blazor
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Configuração do Nginx
COPY src/Portfolio.Web/nginx.conf /etc/nginx/nginx.conf

# Script para substituir URL da API em runtime
COPY src/Portfolio.Web/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

EXPOSE 80

ENTRYPOINT ["/docker-entrypoint.sh"]