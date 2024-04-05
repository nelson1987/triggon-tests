FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/Stargazer.WebApi/Stargazer.WebApi.csproj", "Stargazer.WebApi/"]
COPY ["/Stargazer.Application/Stargazer.Application.csproj", "Stargazer.Application/"]
COPY ["/Stargazer.Domain/Stargazer.Domain.csproj", "Stargazer.Domain/"]
COPY ["/Stargazer.Infrastructure/Stargazer.Infrastructure.csproj", "Stargazer.Infrastructure/"]
COPY ["/Stargazer.Infrastructure.Persistence/Stargazer.Infrastructure.Persistence.csproj", "Stargazer.Infrastructure.Persistence/"]
RUN dotnet restore "./Stargazer.WebApi/Stargazer.WebApi.csproj"
COPY . .
WORKDIR "/src/Stargazer.WebApi"
RUN dotnet build "Stargazer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Stargazer.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stargazer.WebApi.dll"]