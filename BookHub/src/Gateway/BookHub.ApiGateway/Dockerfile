# Étape 1 : image de base pour runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# Forcer ASP.NET à écouter sur le port 80
ENV ASPNETCORE_URLS=http://+:80

# Étape 2 : build de l'application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "BookHub.ApiGateway.csproj"
RUN dotnet publish "BookHub.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Étape 3 : image finale
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BookHub.ApiGateway.dll"]
