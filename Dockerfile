# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
EXPOSE 5000

# instalar curl para healthcheck
RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copia apenas o csproj para cache de restore
COPY src/PeladaPay.API/PeladaPay.API.csproj src/PeladaPay.API/

RUN dotnet restore src/PeladaPay.API/PeladaPay.API.csproj

# copia o restante do código
COPY . .

WORKDIR /src/src/PeladaPay.API

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---------- final ----------
FROM runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PeladaPay.API.dll"]