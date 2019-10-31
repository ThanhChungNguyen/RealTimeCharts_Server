FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["RealTimeCharts_Server/RealTimeCharts_Server.csproj", "RealTimeCharts_Server/"]
RUN dotnet restore "RealTimeCharts_Server/RealTimeCharts_Server.csproj"
COPY . .
WORKDIR "/src/RealTimeCharts_Server"
RUN dotnet build "RealTimeCharts_Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RealTimeCharts_Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RealTimeCharts_Server.dll"]