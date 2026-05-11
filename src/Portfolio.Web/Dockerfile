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

# STAGE 2: Nginx para servir arquivos estáticos
FROM nginx:alpine AS runtime

# Copiar arquivos publicados do Blazor
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Configuração do Nginx para Blazor (suporte a rotas SPA)
COPY src/Portfolio.Web/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]