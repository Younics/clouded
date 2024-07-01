FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared/Clouded.Shared.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj", "Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj"]
COPY ["Core/Repository/Clouded.Core.Repository/Clouded.Core.Repository.csproj", "Core/Repository/Clouded.Core.Repository/Clouded.Core.Repository.csproj"]
COPY ["Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj", "Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj"]
COPY ["Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj", "Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj"]
COPY ["Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj", "Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj"]
COPY ["Platform/Clouded.Platform.Hub/Clouded.Platform.Hub.csproj", "Platform/Clouded.Platform.Hub/Clouded.Platform.Hub.csproj"]
RUN dotnet restore "Platform/Clouded.Platform.Hub/Clouded.Platform.Hub.csproj"
COPY . .
WORKDIR "/src/Platform/Clouded.Platform.Hub"
RUN dotnet build "Clouded.Platform.Hub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clouded.Platform.Hub.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Platform.Hub"]