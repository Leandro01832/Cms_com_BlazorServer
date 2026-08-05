# ESTÁGIO 1: Compilação e Publicação (.NET 9)
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

# Copia os arquivos publicados do estágio de build
COPY --from=build /app/publish .

# Define que o ASP.NET escutará em todas as interfaces na porta informada pelo Render (padrão 10000)
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Executa a DLL do projeto principal
ENTRYPOINT ["dotnet", "BlazorServerCms.dll"]