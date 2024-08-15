FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/License/Clouded.Core.License/Clouded.Core.License.csproj", "Core/License/Clouded.Core.License/Clouded.Core.License.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Api/Clouded.Core.DataSource.Api.csproj", "Core/DataSource/Clouded.Core.DataSource.Api/Clouded.Core.DataSource.Api.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Mysql/Clouded.Core.DataSource.Mysql.csproj", "Core/DataSource/Clouded.Core.DataSource.Mysql/Clouded.Core.DataSource.Mysql.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Postgres/Clouded.Core.DataSource.Postgres.csproj", "Core/DataSource/Clouded.Core.DataSource.Postgres/Clouded.Core.DataSource.Postgres.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj", "Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj"]
COPY ["Auth/Clouded.Auth.Provider/Clouded.Auth.Provider.csproj", "Auth/Clouded.Auth.Provider/Clouded.Auth.Provider.csproj"]
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared.csproj"]
RUN dotnet restore "Auth/Clouded.Auth.Provider/Clouded.Auth.Provider.csproj"
COPY . .
WORKDIR "/src/Auth/Clouded.Auth.Provider"
RUN dotnet build "Clouded.Auth.Provider.csproj" -c Release -o /app/build

# Publishing only linux-x64 architecture 
FROM build AS publish
RUN dotnet publish "Clouded.Auth.Provider.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Auth.Provider"]