# PeladaPay API

API .NET 9 com Clean Architecture, CQRS (MediatR), EF Core/PostgreSQL, Identity + JWT, FluentValidation e Swagger.

## Subir com Docker

```bash
docker compose up --build
```

A API sobe em `http://localhost:8080` e o Swagger em `/swagger`.


## Subir somente o banco local (PostgreSQL)

```bash
docker compose -f docker-compose.db.yml up -d
```

Parar o banco:

```bash
docker compose -f docker-compose.db.yml down
```

## Credenciais seed

- Email: `gestor@peladapay.com`
- Senha: `Pelada123!`
