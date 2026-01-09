FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Finance.Api/Finance.Api.csproj", "Finance.Api/"]
COPY ["Finance.Contracts/Finance.Contracts.csproj", "Finance.Contracts/"]
COPY ["Finance.Application/Finance.Application.csproj", "Finance.Application/"]
COPY ["Finance.Domain/Finance.Domain.csproj", "Finance.Domain/"]
COPY ["Finance.Infrastructure/Finance.Infrastructure.csproj", "Finance.Infrastructure/"]

RUN dotnet restore "Finance.Api/Finance.Api.csproj"

COPY . .
WORKDIR "/src/Finance.Api"

FROM build AS publish
RUN dotnet publish "Finance.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER app
ENTRYPOINT ["dotnet", "Finance.Api.dll"]