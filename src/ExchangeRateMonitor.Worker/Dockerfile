FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/ExchangeRateMonitor.Worker/ExchangeRateMonitor.Worker.csproj", "src/ExchangeRateMonitor.Worker/"]
COPY ["src/ExchangeRateMonitor.Core/ExchangeRateMonitor.Core.csproj", "src/ExchangeRateMonitor.Core/"]
COPY ["src/ExchangeRateMonitor.DAL.EF/ExchangeRateMonitor.DAL.EF.csproj", "src/ExchangeRateMonitor.DAL.EF/"]
COPY ["src/ExchangeRateMonitor.Services/ExchangeRateMonitor.Services.csproj", "src/ExchangeRateMonitor.Services/"]
RUN dotnet restore "src/ExchangeRateMonitor.Worker/ExchangeRateMonitor.Worker.csproj"
COPY . .
WORKDIR "/src/src/ExchangeRateMonitor.Worker"
RUN dotnet build "ExchangeRateMonitor.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExchangeRateMonitor.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExchangeRateMonitor.Worker.dll"]
