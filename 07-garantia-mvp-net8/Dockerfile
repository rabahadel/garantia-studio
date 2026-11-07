# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["GarantIA.csproj", "./"]
RUN dotnet restore "GarantIA.csproj"

COPY . .
RUN dotnet publish "GarantIA.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime (Non-Root Security-by-Design)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

USER $APP_UID
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GarantIA.dll"]
