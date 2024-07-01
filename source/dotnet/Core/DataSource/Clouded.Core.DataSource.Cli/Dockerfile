FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Core/DataSource/Clouded.Core.DataSource.Cli/Clouded.Core.DataSource.Cli.csproj", "Clouded.Core.DataSource.Cli/"]
RUN dotnet restore "Core/DataSource/Clouded.Core.DataSource.Cli/Clouded.Core.DataSource.Cli.csproj"
COPY . .
WORKDIR "/src/Clouded.Core.DataSource.Cli"
RUN dotnet build "Clouded.Core.DataSource.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clouded.Core.DataSource.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Clouded.Core.DataSource.Cli.dll"]
