# -------- BUILD --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia apenas arquivos de projeto para aproveitar cache
COPY src/PeladaPay.API/PeladaPay.API.csproj src/PeladaPay.API/
COPY src/PeladaPay.Application/PeladaPay.Application.csproj src/PeladaPay.Application/
COPY src/PeladaPay.Domain/PeladaPay.Domain.csproj src/PeladaPay.Domain/
COPY src/PeladaPay.Infrastructure/PeladaPay.Infrastructure.csproj src/PeladaPay.Infrastructure/

# Restore das dependências
RUN dotnet restore src/PeladaPay.API/PeladaPay.API.csproj

# Agora copia o restante do código
COPY . .

# Build + Publish
RUN dotnet publish src/PeladaPay.API/PeladaPay.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# -------- RUNTIME --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "PeladaPay.API.dll"]