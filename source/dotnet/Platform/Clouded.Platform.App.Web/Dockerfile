FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update
RUN apt-get install --no-install-recommends -y curl gnupg lsb-release
RUN curl -L https://www.postgresql.org/media/keys/ACCC4CF8.asc | gpg --dearmor | tee /usr/share/keyrings/postgresql-archive-keyring.gpg > /dev/null
RUN echo "deb [arch=amd64 signed-by=/usr/share/keyrings/postgresql-archive-keyring.gpg] http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" | tee /etc/apt/sources.list.d/postgresql.list
RUN apt-get update
RUN apt-get install --no-install-recommends -y \
        postgresql-client-15 \
        postgresql-client-14 \
        postgresql-client-13 \
        postgresql-client-12 \
        postgresql-client-11 \
        postgresql-client-10
RUN apt-get clean all

ENV PG_DIR=/usr/lib/postgresql

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared.csproj"]
COPY ["Core/Repository/Clouded.Core.Repository/Clouded.Core.Repository.csproj", "Core/Repository/Clouded.Core.Repository/Clouded.Core.Repository.csproj"]
COPY ["Core/WebSupport/WebSupport.Library/WebSupport.Library.csproj", "Core/WebSupport/WebSupport.Library/WebSupport.Library.csproj"]
COPY ["Core/Harbor/Harbor.Library/Harbor.Library.csproj", "Core/Harbor/Harbor.Library/Harbor.Library.csproj"]
COPY ["Auth/Clouded.Auth.Client/Clouded.Auth.Client.csproj", "Auth/Clouded.Auth.Client/Clouded.Auth.Client.csproj"]
COPY ["Auth/Clouded.Auth.Shared/Clouded.Auth.Shared.csproj", "Auth/Clouded.Auth.Shared/Clouded.Auth.Shared.csproj"]
COPY ["Function/Clouded.Function.Library/Clouded.Function.Library.csproj", "Function/Clouded.Function.Library/Clouded.Function.Library.csproj"]
COPY ["Function/Clouded.Function.Shared/Clouded.Function.Shared.csproj", "Function/Clouded.Function.Shared/Clouded.Function.Shared.csproj"]
COPY ["Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj", "Platform/Clouded.Platform.Database/Clouded.Platform.Database.csproj"]
COPY ["Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj", "Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj"]
COPY ["Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj", "Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj"]
COPY ["Platform/Clouded.Platform.App.Web/Clouded.Platform.App.Web.csproj", "Platform/Clouded.Platform.App.Web/Clouded.Platform.App.Web.csproj"]
RUN dotnet restore "Platform/Clouded.Platform.App.Web/Clouded.Platform.App.Web.csproj"
COPY . .
WORKDIR "/src/Platform/Clouded.Platform.App.Web"
RUN dotnet build "Clouded.Platform.App.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clouded.Platform.App.Web.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Platform.App.Web"]