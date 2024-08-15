FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Function/Clouded.Function.Shared/Clouded.Function.Shared.csproj", "Function/Clouded.Function.Shared/Clouded.Function.Shared.csproj"]
COPY ["Function/Clouded.Function.Api/Clouded.Function.Api.csproj", "Function/Clouded.Function.Api/Clouded.Function.Api.csproj"]
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared.csproj"]
RUN dotnet restore "Function/Clouded.Function.Api/Clouded.Function.Api.csproj"
COPY . .
WORKDIR "/src/Function/Clouded.Function.Api"
RUN dotnet build "Clouded.Function.Api.csproj" -c Release -o /app/build

# Publishing only linux-x64 architecture
FROM build AS publish
RUN dotnet publish "Clouded.Function.Api.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Function.Api"]