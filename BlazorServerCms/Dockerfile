# Estágio 1: Base para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Estágio 2: SDK para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Substitua "SeuProjeto.csproj" pelo nome real do seu arquivo de projeto
COPY ["BlazorServerCms.csproj", "./"]
RUN dotnet restore "./BlazorServerCms.csproj"
COPY . .
RUN dotnet build "BlazorServerCms.csproj" -c Release -o /app/build

# Estágio 3: Publicação
FROM build AS publish
RUN dotnet publish "BlazorServerCms.csproj" -c Release -o /app/publish

# Estágio 4: Final - Copia apenas o necessário para rodar
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorServerCms.dll"]