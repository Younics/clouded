FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Clouded.Shared/Clouded.Shared.csproj", "Clouded.Shared/Clouded.Shared.csproj"]
COPY ["Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj", "Platform/Clouded.Platform.Models/Clouded.Platform.Models.csproj"]
COPY ["Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj", "Platform/Clouded.Platform.Shared/Clouded.Platform.Shared.csproj"]
COPY ["Platform/Clouded.Platform.Provider/Clouded.Platform.Provider.csproj", "Platform/Clouded.Platform.Provider/Clouded.Platform.Provider.csproj"]
RUN dotnet restore "Platform/Clouded.Platform.Provider/Clouded.Platform.Provider.csproj"
COPY . .
WORKDIR "/src/Platform/Clouded.Platform.Provider"
RUN dotnet build "Clouded.Platform.Provider.csproj" -c Release -o /app/build

# Publishing only linux-x64 architecture
FROM build AS publish
RUN dotnet publish "Clouded.Platform.Provider.csproj" -c Release -o /app/publish -r linux-x64 --self-contained false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish/ .
RUN rm -rf /app/publish/*.pdb

ENTRYPOINT ["./Clouded.Platform.Provider"]