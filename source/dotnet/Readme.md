# Install

## Pre-commit hook
https://github.com/dotnet/format/blob/main/docs/integrations.md#git-pre-commit-hook-to-reformat

## Entity Framework Tool
```bash
dotnet tool install --global dotnet-ef
```

## Docker external network

```bash
docker network create --driver bridge clouded_network 
```

# Migrations

## Create new database migration
Inside folder clouded/source/dotnet run script below
```bash
dotnet ef migrations add "Name of migration" \
--startup-project Platform/Clouded.Platform.WebApp/Clouded.Platform.WebApp.csproj \
--project Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj \
--context CloudedDbContext \
--output-dir Migrations
```

## Apply existing migrations to database
```bash
dotnet ef database update \
--startup-project Platform/Clouded.Platform.WebApp/Clouded.Platform.WebApp.csproj \
--project Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj \
--context CloudedDbContext
```

# Run

## Run and watch for changes
Inside folder with .csproj run script below
```bash
dotnet watch run <ChangeForProjectName>.csproj
```