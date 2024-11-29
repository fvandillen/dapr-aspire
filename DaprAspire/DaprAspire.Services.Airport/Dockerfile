FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Futuretech.Services.Airport/Futuretech.Services.Airport.csproj", "Futuretech.Services.Airport/"]
RUN dotnet restore "Futuretech.Services.Airport/Futuretech.Services.Airport.csproj"
COPY . .
WORKDIR "/src/Futuretech.Services.Airport"
RUN dotnet build "Futuretech.Services.Airport.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Futuretech.Services.Airport.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Futuretech.Services.Airport.dll"]
