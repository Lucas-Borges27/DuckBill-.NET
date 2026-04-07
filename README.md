# DuckBill .NET (ASP.NET Core 8 + EF Core)

## Integrantes
- Bruno Carlos Soares RM 559250 
- Lucas Borges de Souza RM 560027 
- Pedro Henrique Rodrigues RM 560393

## Definição do Projeto

### Visão Geral do Projeto
DuckBill .NET é uma aplicação de gerenciamento financeiro pessoal desenvolvida em ASP.NET Core 8 com Entity Framework Core, utilizando banco de dados Oracle. O sistema permite o controle completo de usuários, categorias de despesas, despesas, ativos financeiros e transações de ativos. Inclui integração com API externa para conversão de moedas em tempo real, relatórios financeiros e uma API REST completa com documentação Swagger, além de observabilidade (health checks, logs estruturados e OpenTelemetry com métricas e tracing).

### Objetivo do Projeto
Este projeto tem como objetivo desenvolver uma aplicação para gerenciamento financeiro pessoal, permitindo o controle de usuários, categorias de despesas e despesas associadas, integrando com banco de dados Oracle para persistência dos dados. Inclui funcionalidades avançadas como busca paginada, ordenação, filtros e links HATEOAS para navegação.

### Escopo
O sistema permitirá:
- Cadastro, consulta, atualização e exclusão de usuários.
- Gerenciamento de categorias de despesas.
- Registro e controle de despesas por usuário e categoria.
- Gerenciamento de ativos financeiros (ações, fundos, etc.) com cotações em tempo real.
- Controle de transações de ativos (compra/venda).
- Conversão de moedas em tempo real via integração com API externa.
- Relatórios financeiros (top 3 gastos do mês).
- Exposição de API REST completa para integração com clientes externos.
- Documentação da API via Swagger.
- Busca avançada com paginação, ordenação e filtros para todos os domínios.
- Links HATEOAS para navegação entre recursos.

### Requisitos Funcionais
- CRUD completo para usuários, categorias, despesas, ativos e transações de ativos.
- Validação de dados de entrada com mensagens de erro apropriadas.
- Tratamento de erros e respostas HTTP adequadas (400, 404, 500).
- Relatórios financeiros (top 3 gastos do mês por usuário).
- Conversão de moedas em tempo real via API externa (AwesomeAPI).
- Autenticação e autorização (a implementar).
- Integração com banco Oracle para persistência de dados.

### Requisitos Não Funcionais
- Arquitetura limpa para separação de responsabilidades.
- Código desacoplado e testável.
- Uso de Entity Framework Core para acesso a dados.
- Documentação da API via Swagger.
- Configuração via arquivos JSON.
- Observabilidade com health checks, logs estruturados, tracing distribuído e métricas Prometheus/OpenTelemetry.
- Testes automatizados separados por camada, seguindo padrão AAA.

## Desenho da Arquitetura

### Clean Architecture
O projeto segue os princípios da Clean Architecture para garantir a separação clara entre as camadas, facilitando manutenção, testes e evolução do sistema.

### Camadas da Aplicação

- **Apresentação (DuckBill.Api):**
  - Contém a API REST, configuração do servidor, mapeamento de endpoints e documentação Swagger.
  - Responsável por expor os serviços da aplicação para clientes externos.

- **Aplicação (DuckBill.Application):**
  - Implementa os serviços e casos de uso do sistema.
  - Contém DTOs para comunicação entre camadas.
  - Define interfaces para repositórios e implementa a lógica de negócios.

- **Domínio (DuckBill.Domain):**
  - Contém os modelos de domínio e regras de negócio.
  - Define as entidades principais do sistema: usuários, categorias, despesas, ativos financeiros (Ativo), cotações (CotacaoAtivo) e transações (TransacaoAtivo).
  - Contém interfaces para repositórios e serviços externos (ICambioService).

- **Infraestrutura (DuckBill.Infrastructure):**
  - Implementa o acesso a dados usando Entity Framework Core com Oracle.
  - Contém os repositórios concretos para todas as entidades.
  - Gerencia migrações de banco de dados.
  - Implementa integrações externas (AwesomeAPI para conversão de moedas).

## Instruções de Instalação e Configuração

### Pré-requisitos
- .NET 8 SDK
- Oracle Database (ou Docker com Oracle XE)
- Git

### Instalação
1. Clone o repositório:
```bash
git clone <repository-url>
cd DuckBill-.NET
```

2. Restaure as dependências:
```bash
cd src/DuckBill.Api
dotnet restore
```

3. Configure a string de conexão do banco de dados no arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Default": "User Id=your_user;Password=your_password;Data Source=your_oracle_datasource;"
  }
}
```

4. Execute as migrações do banco de dados:
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project ../DuckBill.Infrastructure --startup-project .
```

5. Execute a aplicação:
```bash
dotnet run
```

6. Acesse a documentação da API via Swagger em: http://localhost:5000/swagger

### Banco de Dados
- Utiliza Oracle Database para persistência.
- Migrações gerenciadas via Entity Framework Core.

## Monitoramento e Observabilidade (Sprint 3)

### Endpoints de Health Check
- `GET /health/live` - verifica saúde da API (liveness)
- `GET /health/ready` - verifica dependências (DB Oracle + AwesomeApi)
- Resposta em JSON com `status`, `totalDurationMs` e detalhe de cada verificação, incluindo metadados das dependências monitoradas

#### Exemplos
```bash
curl http://localhost:5000/health/live
curl http://localhost:5000/health/ready
```

### Métricas e Tracing
- Métricas expostas em `GET /metrics` (OpenTelemetry + Prometheus exporter)
- Tracing com OpenTelemetry para requisições HTTP (API e chamadas externas), EF Core e chamadas entre camadas (Application Services)
- Métricas customizadas expostas:
  - `duckbill_http_server_request_duration_ms`: tempo de resposta das requisições
  - `duckbill_http_server_requests`: total de requisições HTTP
  - `duckbill_http_server_request_errors`: total de requisições HTTP com erro

#### Como monitorar
1. Inicie a API com `dotnet run` em `src/DuckBill.Api`
2. Consulte `GET /health/live` para saúde básica da API
3. Consulte `GET /health/ready` para validar banco e serviço externo
4. Consulte `GET /metrics` para métricas em formato Prometheus
5. Observe o console para spans OpenTelemetry exportados
6. Consulte os arquivos em `src/DuckBill.Api/logs/` ou `bin/Debug/net8.0/logs/` para logs estruturados
7. Envie opcionalmente o header `X-Correlation-ID` para correlacionar chamadas ponta a ponta

#### Exemplo de coleta de métricas
```bash
curl http://localhost:5000/metrics
```

### Logging Estruturado
- Serilog com logs estruturados no console e em arquivo
- Arquivos em `logs/log-YYYYMMDD.json`
- Correlação de requisições via header `X-Correlation-ID`
- Logs com níveis `Information`, `Warning` e `Error`
- Requests `2xx/3xx` geram `Information`, `4xx` geram `Warning` e `5xx`/exceções geram `Error`
- Em erros não tratados, a resposta inclui `correlationId` e `traceId` para facilitar diagnóstico

### Autenticação (opcional por configuração)
- API Key via header `X-API-KEY`
- Ative em `appsettings.json` com:
```json
{
  "Authentication": {
    "Enabled": true,
    "ApiKey": "sua-chave"
  }
}
```

## Migrações
```bash
dotnet tool install --global dotnet-ef
dotnet add ../DuckBill.Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet ef migrations add Initial --project ../DuckBill.Infrastructure --startup-project .
dotnet ef database update --project ../DuckBill.Infrastructure --startup-project .
```

## Testes Automatizados

### Organização
- `tests/DuckBill.UnitTests`: testes unitários das camadas de Domínio e Aplicação com xUnit, Moq e padrão AAA
- `tests/DuckBill.IntegrationTests`: testes de integração dos endpoints com `WebApplicationFactory`, `CollectionFixture` e banco em memória
- Convenção de nomes adotada: `MetodoTestado_Cenario_ResultadoEsperado`

### Como executar
```bash
dotnet test
```

### Executar por camada
```bash
dotnet test tests/DuckBill.UnitTests/DuckBill.UnitTests.csproj
dotnet test tests/DuckBill.IntegrationTests/DuckBill.IntegrationTests.csproj
```

### Cobertura de testes
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Cenários cobertos
- Testes unitários de criação, atualização, paginação, validações e regras de negócio em `UsuarioService` e `DespesaService`
- Testes unitários básicos das entidades de domínio (`Usuario`, `Categoria`, `Despesa`)
- Testes de integração para autenticação via API Key, CRUD de usuários, health checks, métricas e propagação do `X-Correlation-ID`

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

### Despesas
- `GET /api/despesas` - Lista todas as despesas
- `GET /api/despesas/search?usuarioId={uid}&filter={f}&sort={s}&page={p}&size={sz}` - Busca paginada com filtros e ordenação (HATEOAS)
- `GET /api/despesas/{id}` - Busca despesa por ID
- `POST /api/despesas` - Cria nova despesa
- `PUT /api/despesas/{id}` - Atualiza despesa
- `DELETE /api/despesas/{id}` - Remove despesa

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

## Testando a API

### Exemplos de Requisições

#### GET - Listar Usuários
```bash
curl http://localhost:5000/api/usuarios
```

#### GET - Buscar Usuário por ID
```bash
curl http://localhost:5000/api/usuarios/1
```

#### POST - Criar Nova Categoria
```bash
curl -X POST http://localhost:5000/api/categorias \
  -H "Content-Type: application/json" \
  -d '{"nome":"Nova Categoria"}'
```

#### POST - Criar Novo Usuário
```bash
curl -X POST http://localhost:5000/api/usuarios \
  -H "Content-Type: application/json" \
  -d '{"nome":"João Silva","email":"joao@email.com","senha":"123456"}'
```

## Testes Automatizados (Sprint 3)

### Executar todos os testes
```bash
dotnet test
```

### Executar por projeto
```bash
dotnet test tests/DuckBill.UnitTests/DuckBill.UnitTests.csproj
dotnet test tests/DuckBill.IntegrationTests/DuckBill.IntegrationTests.csproj
```

### Estrutura de testes
- `tests/DuckBill.UnitTests` (Domínio e Aplicação)
- `tests/DuckBill.IntegrationTests` (API com WebApplicationFactory)

### Padrões adotados
- AAA (Arrange, Act, Assert)
- Nomenclatura: `MetodoTestado_Cenario_ResultadoEsperado`
- Uso de Moq para mocks
- Uso de Collection Fixture para compartilhar `WebApplicationFactory` entre testes de integração
- Cobertura de autenticação, sucesso, erro, health checks e métricas

#### GET - Conversão de Moedas
```bash
curl "http://localhost:5000/api/relatorios/cambio?from=USD&to=BRL&valor=100"
```

#### GET - Busca Paginada de Usuários
```bash
curl "http://localhost:5000/api/usuarios/search?filter=João&sort=nome,desc&page=1&size=10"
```

#### PUT - Atualizar Categoria
```bash
curl -X PUT http://localhost:5000/api/categorias/1 \
  -H "Content-Type: application/json" \
  -d '{"nome":"Categoria Atualizada"}'
```

#### DELETE - Remover Usuário
```bash
curl -X DELETE http://localhost:5000/api/usuarios/1
```

# Swagger: http://localhost:5000/swagger
