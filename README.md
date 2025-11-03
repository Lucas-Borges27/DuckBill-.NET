# DuckBill .NET (ASP.NET Core 8 + EF Core)

## Integrantes
- Bruno Carlos Soares RM 559250 
- Lucas Borges de Souza RM 560027 
- Pedro Henrique Rodrigues RM 560393

## Definição do Projeto

### Visão Geral do Projeto
DuckBill .NET é uma aplicação de gerenciamento financeiro pessoal desenvolvida em ASP.NET Core 8 com Entity Framework Core, utilizando banco de dados Oracle. O sistema permite o controle completo de usuários, categorias de despesas, despesas, ativos financeiros e transações de ativos. Inclui integração com API externa para conversão de moedas em tempo real, relatórios financeiros e uma API REST completa com documentação Swagger.

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
    "DefaultConnection": "User Id=your_user;Password=your_password;Data Source=your_oracle_datasource;"
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

## Migrações
```bash
dotnet tool install --global dotnet-ef
dotnet add ../DuckBill.Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet ef migrations add Initial --project ../DuckBill.Infrastructure --startup-project .
dotnet ef database update --project ../DuckBill.Infrastructure --startup-project .
```

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

