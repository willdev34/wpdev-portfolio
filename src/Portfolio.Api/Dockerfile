# ====================================
# Dockerfile - Portfolio.Api
# Multi-stage build para otimizar tamanho
# ====================================

# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto para restaurar dependências
COPY src/Portfolio.Domain/Portfolio.Domain.csproj src/Portfolio.Domain/
COPY src/Portfolio.Application/Portfolio.Application.csproj src/Portfolio.Application/
COPY src/Portfolio.Infrastructure/Portfolio.Infrastructure.csproj src/Portfolio.Infrastructure/
COPY src/Portfolio.Api/Portfolio.Api.csproj src/Portfolio.Api/

# Restaurar dependências
RUN dotnet restore src/Portfolio.Api/Portfolio.Api.csproj

# Copiar todo o código fonte
COPY src/ src/

# Build e publish
WORKDIR /src/src/Portfolio.Api
RUN dotnet publish Portfolio.Api.csproj -c Release -o /app/publish

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar arquivos publicados
COPY --from=build /app/publish .

# Porta da aplicação
EXPOSE 8080

# Variável de ambiente para produção
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Executar a aplicação
ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]