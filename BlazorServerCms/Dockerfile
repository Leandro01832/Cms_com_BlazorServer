# ESTÁGIO 1: Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Copia os arquivos de projeto primeiro para restaurar dependências (mais rápido)
COPY ["BlazorServerCms/BlazorServerCms.csproj", "BlazorServerCms/"]
COPY ["business/business.csproj", "business/"]

# 2. Restaura as dependências do NuGet
RUN dotnet restore "BlazorServerCms/BlazorServerCms.csproj"

# 3. Copia o restante do código fonte de todas as pastas
COPY . .

# 4. Compila e Publica o projeto para a pasta /app/publish
WORKDIR "/src/BlazorServerCms"
RUN dotnet publish "BlazorServerCms.csproj" -c Release -o /app/publish

# ESTÁGIO 2: Final (Execução)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# 5. Copia apenas os arquivos necessários do estágio de build
COPY --from=build /app/publish .

# 6. Cria a pasta do banco (se necessário) e dá permissão total para o SQLite
# Isso resolve erros de "Access Denied" no banco de dados
RUN chmod -R 777 /app

# 7. Define a porta padrão interna como 8080 (padrão do .NET 9)
ENV ASPNETCORE_URLS=http://+:8080

# 8. Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "BlazorServerCms.dll"]
