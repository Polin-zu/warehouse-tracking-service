FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Warehouse.sln .
COPY Warehouse.Domain/Warehouse.Domain.csproj Warehouse.Domain/
COPY Warehouse.Application/Warehouse.Application.csproj Warehouse.Application/
COPY Warehouse.Infrastructure/Warehouse.Infrastructure.csproj Warehouse.Infrastructure/
COPY Warehouse.Api/Warehouse.Api.csproj Warehouse.Api/
COPY Warehouse.Domain.Tests/Warehouse.Domain.Tests.csproj Warehouse.Domain.Tests/
COPY Warehouse.Application.Tests/Warehouse.Application.Tests.csproj Warehouse.Application.Tests/

RUN dotnet restore Warehouse.sln

COPY . .

RUN dotnet publish Warehouse.Api/Warehouse.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Warehouse.Api.dll"]