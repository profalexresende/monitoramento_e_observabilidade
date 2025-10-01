# Monitoramento e Observabilidade em ASP.NET Core Web API

![.NET](https://img.shields.io/badge/.NET-8-blue)
![MongoDB](https://img.shields.io/badge/MongoDB-Connected-green)
![Health](https://img.shields.io/badge/Health-Healthy-brightgreen)

Este projeto é uma **API RESTful em ASP.NET Core 8** que demonstra conceitos de **monitoramento e observabilidade**, utilizando:

- **Health Checks** para verificar a saúde da aplicação e do MongoDB.
- **Tracing** com OpenTelemetry, exportável para console ou Jaeger.
- **MongoDB** como banco de dados de exemplo.
- **Swagger** para testar os endpoints.

---

## 🔹 Funcionalidades

1. **API de Produtos**
   - `GET /api/Produtos` → Lista todos os produtos.
   - `POST /api/Produtos` → Cria um novo produto.
   
2. **Health Check**
   - Endpoint: `/health`
   - Verifica:
     - Conexão com o MongoDB
     - Status interno da API
   - Retorna JSON estruturado com detalhes de cada verificação.

3. **Observabilidade**
   - Tracing via OpenTelemetry.
   - Exporta para:
     - Console
     - Jaeger (opcional)
   - Cada requisição gera `TraceId` e `SpanId`.

---

## 🔹 Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- MongoDB
- OpenTelemetry
- Swagger
- AspNetCore.HealthChecks.MongoDb
- AspNetCore.HealthChecks.UI.Client

---

## 🔹 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/)
- (Opcional) [Jaeger](https://www.jaegertracing.io/) para visualização de traces

---

## 🔹 Configuração

1. Clone o repositório:

```bash
git clone https://github.com/seuusuario/projeto-monitoramento.git
cd projeto-monitoramento
```

2. Configure o `appsettings.json` com sua conexão MongoDB e Jaeger (opcional):

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://usuario:senha@localhost:27017",
    "Database": "meuBanco"
  },
  "Jaeger": {
    "Host": "localhost",
    "Port": "6831"
  }
}
```

3. Instale os pacotes (caso ainda não estejam instalados):

```bash
dotnet restore
dotnet add package AspNetCore.HealthChecks.MongoDb
dotnet add package AspNetCore.HealthChecks.UI.Client
dotnet add package OpenTelemetry.Extensions.Hosting
dotnet add package OpenTelemetry.Instrumentation.AspNetCore
dotnet add package OpenTelemetry.Instrumentation.Http
dotnet add package OpenTelemetry.Exporter.Jaeger
```

---

## 🔹 Rodando o projeto

```bash
dotnet run
```

- Swagger: [https://localhost:5235/swagger](https://localhost:5235/swagger)
- Health Check: [https://localhost:5235/health](https://localhost:5235/health)

> Teste colocando uma string de conexão MongoDB errada para ver a API retornar `Unhealthy`.

---

## 🔹 Observabilidade e Tracing

- Cada requisição gera **TraceId** e **SpanId**.
- Console exibirá os traces, ou configure **Jaeger** para visualização gráfica.
- Pode ser usado para aprendizado de **tracing distribuído**.

---

## 🔹 Estrutura do projeto

```
/Controllers
    ProdutosController.cs
/Models
    Produto.cs
Program.cs
appsettings.json
README.md
```

---

## 🔹 Conceitos abordados

- **Monitoramento:** verificar se a aplicação e o banco estão funcionando.
- **Observabilidade:** entender o comportamento da aplicação via logs, métricas e tracing.
- **Health Checks:** endpoints que retornam `Healthy` ou `Unhealthy`.
- **Tracing:** rastreamento de requisições com OpenTelemetry/Jaeger.

---

## 🔹 Licença

Este projeto é **open source** e pode ser usado para aprendizado e testes.

---

## 🔹 Status de Health (exemplo de badges)

- ![Health](https://img.shields.io/badge/Health-Healthy-brightgreen) → API saudável
- ![MongoDB](https://img.shields.io/badge/MongoDB-Connected-green) → Conexão com MongoDB ok

> Para atualizar os badges, você pode configurar integração com CI/CD ou scripts de verificação do `/health`.
