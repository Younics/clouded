FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Core/DataSource/Clouded.Core.DataSource.Api/Clouded.Core.DataSource.Api.csproj", "Core/DataSource/Clouded.Core.DataSource.Api/Clouded.Core.DataSource.Api.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Mysql/Clouded.Core.DataSource.Mysql.csproj", "Core/DataSource/Clouded.Core.DataSource.Mysql/Clouded.Core.DataSource.Mysql.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Postgres/Clouded.Core.DataSource.Postgres.csproj", "Core/DataSource/Clouded.Core.DataSource.Postgres/Clouded.Core.DataSource.Postgres.csproj"]
COPY ["Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj", "Core/DataSource/Clouded.Core.DataSource.Shared/Clouded.Core.DataSource.Shared.csproj"]
COPY ["Admin/Clouded.Admin.Provider/Clouded.Admin.Provider.csproj", "Admin/Clouded.Admin.Provider/Clouded.Admin.Provider.csproj"]
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared.csproj"]
RUN dotnet restore "Admin/Clouded.Admin.Provider/Clouded.Admin.Provider.csproj"
COPY . .
WORKDIR "/src/Admin/Clouded.Admin.Provider"
RUN dotnet build "Clouded.Admin.Provider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Clouded.Admin.Provider.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Admin.Provider"]