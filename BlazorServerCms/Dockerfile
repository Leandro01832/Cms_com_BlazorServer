# ESTÁGIO 1: Compilação e Publicação
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura as dependências
COPY ["BlazorServerCms/BlazorServerCms.csproj", "BlazorServerCms/"]
COPY ["business/business.csproj", "business/"]
RUN dotnet restore "BlazorServerCms/BlazorServerCms.csproj"

# Copia o restante do código e publica
COPY . .
WORKDIR "/src/BlazorServerCms"
RUN dotnet publish "BlazorServerCms.csproj" -c Release -o /app/publish

# ESTÁGIO 2: Execução
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
# Copia apenas os arquivos publicados do estágio anterior
COPY --from=build /app/publish .

# Define a porta e o comando de início
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "BlazorServerCms.dll"]
