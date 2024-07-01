FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Core/WebSupport/WebSupport.Library.Cli/WebSupport.Library.Cli.csproj", "WebSupport.Library.Cli/"]
RUN dotnet restore "Core/WebSupport/WebSupport.Library.Cli/WebSupport.Library.Cli.csproj"
COPY . .
WORKDIR "/src/WebSupport.Library.Cli"
RUN dotnet build "WebSupport.Library.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSupport.Library.Cli.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSupport.Library.Cli.dll"]
