# DuckBill .NET - Sistema de Gerenciamento Financeiro Pessoal

## 📋 Integrantes do Grupo
- **Nome:** _____________________ **RM:** _____
- **Nome:** _____________________ **RM:** _____
- **Nome:** _____________________ **RM:** _____

## 📖 Sobre o Projeto

### Visão Geral
DuckBill .NET é uma aplicação completa de gerenciamento financeiro pessoal desenvolvida em **ASP.NET Core 8** seguindo os princípios de **Clean Architecture**. O sistema oferece controle completo de usuários, categorias de despesas, despesas, ativos financeiros e transações de ativos, com integração a múltiplos bancos de dados (Oracle e MongoDB) e API externa para conversão de moedas em tempo real.

### Tecnologias Utilizadas
- **.NET 8** - Framework principal
- **ASP.NET Core Minimal API** - API REST
- **Entity Framework Core** - ORM para Oracle
- **Oracle Database** - Banco de dados relacional principal
- **MongoDB** - Banco de dados NoSQL para despesas
- **Serilog** - Logging estruturado
- **OpenTelemetry** - Observabilidade (métricas e tracing)
- **Swagger/OpenAPI** - Documentação da API
- **xUnit + Moq** - Testes automatizados
- **Docker** - Containerização

### Arquitetura
O projeto segue **Clean Architecture** com 4 camadas bem definidas:

```
DuckBill-.NET/
├── src/
│   ├── DuckBill.Domain/          # Entidades e interfaces
│   ├── DuckBill.Application/     # Serviços e DTOs
│   ├── DuckBill.Infrastructure/  # Repositórios e integrações externas
│   └── DuckBill.Api/             # Endpoints e configuração
└── tests/
    ├── DuckBill.UnitTests/       # Testes unitários
    └── DuckBill.IntegrationTests/ # Testes de integração
```

### Funcionalidades Principais
- ✅ CRUD completo para Usuários, Categorias, Despesas, Ativos e Transações
- ✅ Persistência dual: Oracle (relacional) + MongoDB (NoSQL para despesas)
- ✅ Busca avançada com paginação, ordenação e filtros
- ✅ Conversão de moedas em tempo real (AwesomeAPI)
- ✅ Relatórios financeiros (top 3 gastos do mês)
- ✅ Health checks (liveness e readiness)
- ✅ Métricas Prometheus e tracing OpenTelemetry
- ✅ Logging estruturado com Serilog
- ✅ Autenticação via API Key (opcional)
- ✅ Documentação Swagger/OpenAPI
- ✅ Containerização com Docker

## 🚀 Como Executar o Projeto

### Pré-requisitos
- **.NET 8 SDK** instalado
- Acesso ao **Oracle Database** (oracle.fiap.com.br)
- **MongoDB** (local ou Atlas) - opcional
- **Docker** (para execução containerizada) - opcional

### Variáveis de Ambiente Necessárias

O projeto utiliza variáveis de ambiente para configurações sensíveis:

| Variável | Descrição | Obrigatória | Exemplo |
|----------|-----------|-------------|---------|
| `ORACLE_CONNECTION_STRING` | String de conexão Oracle | ✅ Sim | `User Id=RM12345;Password=senha;Data Source=oracle.fiap.com.br:1521/ORCL` |
| `API_KEY` | Chave de autenticação da API | ⚠️ Se auth habilitada | `minha-chave-secreta-123` |
| `MONGODB_URI` | URI de conexão MongoDB | ⚠️ Para endpoints Mongo | `mongodb://localhost:27017` ou `mongodb+srv://user:pass@cluster.mongodb.net` |

### Execução Local

1. **Clone o repositório:**
```bash
git clone <repository-url>
cd DuckBill-.NET
```

2. **Configure as variáveis de ambiente:**

**Linux/macOS:**
```bash
export ORACLE_CONNECTION_STRING="User Id=RM12345;Password=senha;Data Source=oracle.fiap.com.br:1521/ORCL"
export MONGODB_URI="mongodb://localhost:27017"
export API_KEY="minha-chave-secreta"
```

**Windows (PowerShell):**
```powershell
$env:ORACLE_CONNECTION_STRING="User Id=RM12345;Password=senha;Data Source=oracle.fiap.com.br:1521/ORCL"
$env:MONGODB_URI="mongodb://localhost:27017"
$env:API_KEY="minha-chave-secreta"
```

3. **Restaure as dependências:**
```bash
dotnet restore
```

4. **Execute as migrações do banco de dados Oracle:**
```bash
dotnet ef database update --project src/DuckBill.Infrastructure --startup-project src/DuckBill.Api
```

5. **Execute a aplicação:**
```bash
cd src/DuckBill.Api
dotnet run
```

6. **Acesse a aplicação:**
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Health Check (Live): http://localhost:5000/health/live
- Health Check (Ready): http://localhost:5000/health/ready
- Métricas: http://localhost:5000/metrics

### Execução com Docker

1. **Build da imagem:**
```bash
docker build -t duckbill-api .
```

2. **Execute o container:**
```bash
docker run -d -p 5000:5000 \
  -e ORACLE_CONNECTION_STRING="User Id=RM12345;Password=senha;Data Source=oracle.fiap.com.br:1521/ORCL" \
  -e MONGODB_URI="mongodb://host.docker.internal:27017" \
  -e API_KEY="minha-chave-secreta" \
  --name duckbill \
  duckbill-api
```

3. **Verifique os logs:**
```bash
docker logs -f duckbill
```

4. **Acesse:** http://localhost:5000/swagger

## 🧪 Testes Automatizados

O projeto possui cobertura completa de testes seguindo o padrão **AAA (Arrange, Act, Assert)**.

### Estrutura de Testes
```
tests/
├── DuckBill.UnitTests/          # Testes unitários (Domain + Application)
│   ├── Domain/                  # Testes de entidades
│   └── Application/             # Testes de serviços
└── DuckBill.IntegrationTests/   # Testes de integração (API)
    ├── ApiFactory.cs            # WebApplicationFactory customizada
    └── *EndpointsTests.cs       # Testes de endpoints
```

### Executar Todos os Testes
```bash
dotnet test
```

### Executar por Projeto
```bash
# Testes unitários
dotnet test tests/DuckBill.UnitTests/DuckBill.UnitTests.csproj

# Testes de integração
dotnet test tests/DuckBill.IntegrationTests/DuckBill.IntegrationTests.csproj
```

### Cobertura de Código
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Cenários Testados
- ✅ Validações de entidades de domínio
- ✅ Lógica de negócio nos serviços
- ✅ CRUD completo de todos os endpoints
- ✅ Autenticação via API Key
- ✅ Health checks (live e ready)
- ✅ Métricas e observabilidade
- ✅ Tratamento de erros
- ✅ Paginação e filtros

## 📊 Observabilidade

### Health Checks
```bash
# Verifica se a API está rodando
curl http://localhost:5000/health/live

# Verifica dependências (Oracle + AwesomeAPI)
curl http://localhost:5000/health/ready
```

### Métricas Prometheus
```bash
# Exporta métricas em formato Prometheus
curl http://localhost:5000/metrics
```

**Métricas disponíveis:**
- `duckbill_http_server_request_duration_ms` - Tempo de resposta
- `duckbill_http_server_requests` - Total de requisições
- `duckbill_http_server_request_errors` - Total de erros

### Logging Estruturado
- Logs em JSON via **Serilog**
- Arquivos em `logs/log-YYYYMMDD.json`
- Correlação via header `X-Correlation-ID`
- Níveis: `Information`, `Warning`, `Error`

### Tracing Distribuído
- **OpenTelemetry** para tracing
- Instrumentação automática de HTTP, EF Core e Application Services
- Spans exportados no console

## 🔐 Autenticação (Opcional)

Para habilitar autenticação via API Key:

1. Configure no `appsettings.json`:
```json
{
  "Authentication": {
    "Enabled": true,
    "ApiKey": "sua-chave-secreta"
  }
}
```

2. Ou use variável de ambiente:
```bash
export API_KEY="sua-chave-secreta"
```

3. Envie o header em todas as requisições:
```bash
curl -H "X-API-KEY: sua-chave-secreta" http://localhost:5000/api/usuarios
```

**Nota:** Endpoints `/health/*`, `/metrics` e `/swagger` não requerem autenticação.

## Endpoints da API

### Usuários
- `GET /api/usuarios` - Lista todos os usuários
- `GET /api/usuarios/search?filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/usuarios/{id}` - Busca usuário por ID
- `POST /api/usuarios` - Cria novo usuário
- `PUT /api/usuarios/{id}` - Atualiza usuário
- `DELETE /api/usuarios/{id}` - Remove usuário

### Categorias
- `GET /api/categorias` - Lista todas as categorias
- `GET /api/categorias/search?filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/categorias/{id}` - Busca categoria por ID
- `POST /api/categorias` - Cria nova categoria
- `PUT /api/categorias/{id}` - Atualiza categoria
- `DELETE /api/categorias/{id}` - Remove categoria

### Despesas (Oracle)
- `GET /api/despesas` - Lista todas as despesas
- `GET /api/despesas/search?usuarioId={uid}&filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/despesas/{id}` - Busca despesa por ID
- `POST /api/despesas` - Cria nova despesa
- `PUT /api/despesas/{id}` - Atualiza despesa
- `DELETE /api/despesas/{id}` - Remove despesa

### Despesas (MongoDB) 🆕
- `GET /api/despesas/mongo` - Lista todas as despesas do MongoDB
- `POST /api/despesas/mongo` - Cria nova despesa no MongoDB

### Ativos
- `GET /api/ativos` - Lista todos os ativos
- `GET /api/ativos/search?filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/ativos/{id}` - Busca ativo por ID
- `POST /api/ativos` - Cria novo ativo
- `PUT /api/ativos/{id}` - Atualiza ativo
- `DELETE /api/ativos/{id}` - Remove ativo

### Transações de Ativos
- `GET /api/transacoes-ativo` - Lista todas as transações
- `GET /api/transacoes-ativo/search?usuarioId={uid}&filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/transacoes-ativo/{id}` - Busca transação por ID
- `POST /api/transacoes-ativo` - Cria nova transação
- `PUT /api/transacoes-ativo/{id}` - Atualiza transação
- `DELETE /api/transacoes-ativo/{id}` - Remove transação

### Relatórios
- `GET /api/relatorios/top3-gastos?usuarioId={id}&mes={m}&ano={a}&moeda={moeda}` - Top 3 gastos do mês
- `GET /api/relatorios/cambio?from={moeda}&to={moeda}&valor={valor}` - Conversão de moedas

## 📝 Exemplos de Uso da API

### Usuários
```bash
# Listar todos
curl http://localhost:5000/api/usuarios

# Buscar por ID
curl http://localhost:5000/api/usuarios/1

# Criar novo
curl -X POST http://localhost:5000/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{"nome":"João Silva","email":"joao@email.com","senha":"123456"}'

# Atualizar
curl -X PUT http://localhost:5000/api/usuarios/1 \
  -H "Content-Type: application/json" \
  -d '{"nome":"João Silva Jr","email":"joao@email.com","senha":"123456"}'

# Deletar
curl -X DELETE http://localhost:5000/api/usuarios/1
```

### Despesas (Oracle)
```bash
# Criar despesa
curl -X POST http://localhost:5000/api/despesas \
  -H "Content-Type: application/json" \
  -d '{"usuarioId":1,"categoriaId":1,"valor":150.50,"moeda":"BRL","dataCompra":"2024-01-15","descricao":"Supermercado"}'
```

### Despesas (MongoDB) 🆕
```bash
# Listar despesas do MongoDB
curl http://localhost:5000/api/despesas/mongo

# Criar despesa no MongoDB
curl -X POST http://localhost:5000/api/despesas/mongo \
  -H "Content-Type: application/json" \
  -d '{"usuarioId":1,"categoriaId":1,"valor":150.50,"moeda":"BRL","dataCompra":"2024-01-15","descricao":"Supermercado"}'
```

### Relatórios
```bash
# Top 3 gastos do mês
curl "http://localhost:5000/api/relatorios/top3-gastos?usuarioId=1&mes=1&ano=2024&moeda=BRL"

# Conversão de moedas
curl "http://localhost:5000/api/relatorios/cambio?from=USD&to=BRL&valor=100"
```

### Busca Paginada
```bash
# Buscar usuários com filtro e ordenação
curl "http://localhost:5000/api/usuarios/search?filter=João&sort=nome,desc&page=1&size=10"
```

## 📚 Documentação Adicional

- **Swagger UI:** http://localhost:5000/swagger
- **Health Check (Live):** http://localhost:5000/health/live
- **Health Check (Ready):** http://localhost:5000/health/ready
- **Métricas Prometheus:** http://localhost:5000/metrics

---

**Desenvolvido com ❤️ para FIAP - Challenge Sprint 4**
