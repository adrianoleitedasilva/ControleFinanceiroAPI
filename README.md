# Controle Financeiro — API
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=csharp&logoColor=white) ![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens) ![SQLite](https://img.shields.io/badge/sqlite-%2307405e.svg?style=for-the-badge&logo=sqlite&logoColor=white) ![ControleFinanceiroAPI](https://img.shields.io/badge/Controle_Financeiro_API-5d576b?style=for-the-badge&logo=quicklook&logoColor=white)

API REST para gerenciamento de finanças pessoais, construída com **C# / .NET 8** e **Minimal APIs**.

## Tecnologias

| Camada | Tecnologias |
|--------|-------------|
| API | ASP.NET Core 8 · Minimal APIs · JWT |
| Banco de Dados | SQLite · Entity Framework Core 8 |
| Segurança | BCrypt · JWT Bearer |
| Documentação | Swagger / OpenAPI |

## Arquitetura

```
src/
├── ControleFinanceiro.Domain/          # Entidades, enums, interfaces de repositório
├── ControleFinanceiro.Application/     # DTOs, interfaces de serviço, lógica de negócio
├── ControleFinanceiro.Infrastructure/  # EF Core, repositórios, JWT service
└── ControleFinanceiro.API/             # Minimal API endpoints, Program.cs
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Como executar

```bash
# Clonar / acessar o projeto
cd src/ControleFinanceiro.API

# Restaurar dependências e executar
dotnet run
```

A API estará disponível em `http://localhost:5000` e o Swagger em `http://localhost:5000` (página inicial).

## Endpoints

### Autenticação
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/register` | Cadastrar novo usuário |
| POST | `/api/auth/login` | Login e obtenção do token JWT |

### Categorias (requer JWT)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/categories` | Listar todas as categorias |
| GET | `/api/categories/{id}` | Buscar categoria por ID |
| POST | `/api/categories` | Criar categoria personalizada |
| PUT | `/api/categories/{id}` | Atualizar categoria |
| DELETE | `/api/categories/{id}` | Excluir categoria |

### Transações (requer JWT)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/transactions` | Listar transações (com filtros e paginação) |
| GET | `/api/transactions/{id}` | Buscar transação por ID |
| POST | `/api/transactions` | Criar transação |
| PUT | `/api/transactions/{id}` | Atualizar transação |
| DELETE | `/api/transactions/{id}` | Excluir transação |

**Query params para GET /api/transactions:**
- `categoryId` — filtrar por categoria
- `type` — `Income` ou `Expense`
- `startDate` / `endDate` — período (formato ISO 8601)
- `search` — busca em descrição/notas
- `page` / `pageSize` — paginação

### Dashboard (requer JWT)
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/dashboard` | Resumo financeiro + transações recentes |
| GET | `/api/dashboard/reports/monthly/{year}/{month}` | Relatório mensal detalhado |

## Configuração JWT

Edite `src/ControleFinanceiro.API/appsettings.json`:

```json
{
  "Jwt": {
    "Key": "sua-chave-secreta-com-pelo-menos-32-caracteres!",
    "Issuer": "ControleFinanceiroAPI",
    "Audience": "ControleFinanceiroClients",
    "ExpirationDays": "7"
  }
}
```

> **Importante:** em produção, use variáveis de ambiente ou User Secrets para a `Jwt:Key`.

## Categorias padrão

O banco é populado automaticamente com 12 categorias padrão (4 de receita + 8 de despesa):

**Receita:** Salário · Freelance · Investimentos · Outros  
**Despesa:** Alimentação · Transporte · Moradia · Saúde · Educação · Lazer · Vestuário · Outros

## Exemplo de uso

```bash
# 1. Registrar usuário
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"João","email":"joao@email.com","password":"senha123"}'

# 2. Criar transação (usar token retornado acima)
curl -X POST http://localhost:5000/api/transactions \
  -H "Authorization: Bearer SEU_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Salário de Janeiro",
    "amount": 5000.00,
    "date": "2026-01-05T00:00:00Z",
    "type": "Income",
    "categoryId": "10000000-0000-0000-0000-000000000001",
    "isRecurring": true
  }'

# 3. Ver dashboard
curl http://localhost:5000/api/dashboard \
  -H "Authorization: Bearer SEU_TOKEN"
```
