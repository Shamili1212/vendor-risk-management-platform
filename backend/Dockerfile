FROM gcc:14 AS risk
WORKDIR /risk
COPY risk-engine/src/risk_engine.c .
RUN gcc -std=c11 -Wall -Wextra -Werror -O2 risk_engine.c -o risk_engine

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY backend/src/VendorRisk.Api/VendorRisk.Api.csproj backend/src/VendorRisk.Api/
RUN dotnet restore backend/src/VendorRisk.Api/VendorRisk.Api.csproj
COPY backend/src/VendorRisk.Api backend/src/VendorRisk.Api
RUN dotnet publish backend/src/VendorRisk.Api/VendorRisk.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=risk /risk/risk_engine /app/risk_engine
EXPOSE 8080
ENTRYPOINT ["dotnet", "VendorRisk.Api.dll"]
