# Minimal API - Sistema de Gerenciamento de Veículos

Uma API simples e eficiente construída com .NET 9 para gerenciar veículos e administradores. Este projeto demonstra princípios de arquitetura limpa, autenticação JWT e integração do Entity Framework Core com MySQL.

## 🚀 Funcionalidades

- **Gerenciamento de Veículos**: Operações CRUD completas para veículos
- **Gerenciamento de Administradores**: Autenticação e autorização de usuários
- **Autenticação JWT**: Autenticação segura baseada em tokens
- **Autorização por Perfis**: Diferentes permissões para perfis Admin e Editor
- **Integração com Banco de Dados**: MySQL com Entity Framework Core
- **Documentação da API**: Documentação integrada com Swagger/Scalar
- **Testes Unitários**: Cobertura abrangente de testes com MSTest

## 🛠️ Tecnologias Utilizadas

- **.NET 9**: Framework mais recente para APIs de alta performance
- **Entity Framework Core**: ORM para operações de banco de dados
- **MySQL**: Banco de dados relacional
- **JWT Bearer Authentication**: Mecanismo de autenticação segura
- **Scalar**: Documentação moderna de API
- **MSTest**: Framework de testes unitários

## 📁 Estrutura do Projeto

```
API/
├── Domain/
│   ├── DTOs/                 # Objetos de Transferência de Dados
│   ├── Entities/             # Entidades do banco de dados
│   ├── Enums/               # Enumerações
│   ├── Interfaces/          # Contratos de serviços
│   ├── ModelViews/          # Modelos de resposta
│   └── Service/             # Implementação da lógica de negócio
├── Infrastructure/
│   └── Db/                  # Contexto e configuração do banco
├── Migrations/              # Migrações do banco de dados
└── Program.cs               # Ponto de entrada da aplicação

Test/
├── Domain/
│   ├── Entities/            # Testes de entidades
│   └── Services/            # Testes de serviços
├── Helpers/                 # Utilitários de teste
├── Mocks/                   # Implementações mock
└── Requests/                # Testes de integração da API
```

## 🔧 Configuração e Instalação

### Pré-requisitos

- .NET 9 SDK
- MySQL Server
- Visual Studio 2022 ou VS Code

### Passos da Instalação

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd minimal-api
   ```

2. **Configure a Conexão com o Banco de Dados**
   
   Atualize a string de conexão no `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "mysql": "Server=localhost;Database=minimal_api;Uid=admin;Pwd=1234;"
     }
   }
   ```

3. **Instale as Dependências**
   ```bash
   dotnet restore
   ```

4. **Execute as Migrações do Banco**
   ```bash
   dotnet ef database update
   ```

5. **Execute a Aplicação**
   ```bash
   dotnet run
   ```

A API estará disponível em `http://localhost:5278` e `https://localhost:7269`.

## 📚 Documentação da API

Acesse a documentação interativa da API em:
- **Interface Scalar**: `/docs`
- **Especificação OpenAPI**: `/openapi/v1.json`

## 🔐 Autenticação

A API utiliza autenticação JWT com as seguintes credenciais padrão de administrador:
- **Email**: `admin@admin.com`
- **Senha**: `1234`

### Perfis e Permissões

- **Admin**: Acesso completo a todos os endpoints incluindo gerenciamento de usuários e CRUD de veículos
- **Editor**: Acesso apenas às operações de veículos

## 📋 Endpoints da API

### Autenticação
- `POST /admin/login` - Autenticar e receber token JWT

### Gerenciamento de Administradores
- `GET /admin` - Listar todos os administradores (Apenas Admin)
- `GET /admin/{id}` - Obter administrador por ID (Apenas Admin)
- `POST /admin/create` - Criar novo administrador (Apenas Admin)

### Gerenciamento de Veículos
- `GET /vehicles` - Listar todos os veículos com paginação e filtros (Editor/Admin)
- `GET /vehicles/{id}` - Obter veículo por ID (Editor/Admin)
- `POST /vehicles` - Criar novo veículo (Editor/Admin)
- `PUT /vehicles/{id}` - Atualizar veículo (Apenas Admin)
- `DELETE /vehicles/{id}` - Excluir veículo (Apenas Admin)

## 🧪 Testes

Execute a suíte de testes:

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Categorias de Teste

- **Testes Unitários**: Teste das camadas de entidade e serviço
- **Testes de Integração**: Teste dos endpoints da API com dependências mock
- **Testes de Banco**: Validação do padrão repository

## 🏗️ Arquitetura

O projeto segue os princípios da **Arquitetura Limpa**:

- **Camada de Domínio**: Contém entidades, DTOs, interfaces e lógica de negócio
- **Camada de Infraestrutura**: Contexto do banco e dependências externas
- **Camada de Apresentação**: Endpoints da API e manipulação de requisições/respostas

### Padrões de Design Principais

- **Padrão Repository**: Acesso a dados abstraído através de interfaces de serviço
- **Padrão DTO**: Separação de entidades internas dos contratos da API
- **Camada de Serviço**: Encapsulamento da lógica de negócio

## 🔧 Configuração

### Variáveis de Ambiente

- `ASPNETCORE_ENVIRONMENT`: Definir como `Development`, `Testing`, ou `Production`
- `JwtSettings:Key`: Chave de assinatura JWT (configurada no appsettings.json)

### Configuração do Banco de Dados

A aplicação utiliza MySQL com Entity Framework Core. O esquema do banco é gerenciado através de migrações localizadas na pasta `Migrations/`.

### Dados Iniciais

A aplicação inclui dados iniciais:
- Usuário administrador padrão: `admin@admin.com` / `1234`

## 🚀 Implantação

### Considerações para Produção

1. **Segurança**: Atualize a chave secreta JWT e credenciais do banco
2. **HTTPS**: Certifique-se de que os certificados SSL/TLS estão configurados adequadamente
3. **Banco de Dados**: Use pool de conexões e indexação adequada
4. **Logging**: Configure logging estruturado para monitoramento em produção

### Suporte ao Docker

Para containerizar a aplicação:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["minimal-api.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "minimal-api.dll"]
```

## 🤝 Contribuindo

1. Faça um fork do repositório
2. Crie uma branch para sua funcionalidade (`git checkout -b feature/funcionalidade-incrivel`)
3. Commit suas mudanças (`git commit -m 'Adiciona funcionalidade incrível'`)
4. Push para a branch (`git push origin feature/funcionalidade-incrivel`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo LICENSE para detalhes.

