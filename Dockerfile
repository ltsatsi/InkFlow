# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PresentationLayer/PresentationLayer.csproj", "PresentationLayer/"]
COPY ["DomainLayer/DomainLayer.csproj", "DomainLayer/"]
COPY ["RepositoryLayer/RepositoryLayer.csproj", "RepositoryLayer/"]
COPY ["ServiceLayer/ServiceLayer.csproj", "ServiceLayer/"]

RUN dotnet restore "PresentationLayer/PresentationLayer.csproj"

COPY . .
RUN dotnet publish "PresentationLayer/PresentationLayer.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "PresentationLayer.dll"]