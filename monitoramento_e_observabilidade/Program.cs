// Importa as bibliotecas necessárias para monitoramento, verificação de saúde, MongoDB e rastreamento
using HealthChecks.MongoDb;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Cria o construtor da aplicação web
var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço de controladores (Controllers) à aplicação
builder.Services.AddControllers();

// ------------ Health Checks ------------
// Adiciona verificações de saúde à aplicação
builder.Services.AddHealthChecks()
    // Verificação simples para saber se a API está funcionando
    .AddCheck("self", () => HealthCheckResult.Healthy("API OK"))
    // Verificação de saúde do MongoDB
    .AddMongoDb(
        sp =>
        {
            // Obtém a string de conexão do MongoDB das configurações
            var cs = builder.Configuration["MongoDb:ConnectionString"];
            return new MongoClient(cs);
        },
        name: "mongodb", // Nome da verificação
        timeout: TimeSpan.FromSeconds(5), // Tempo limite de 5 segundos
        tags: new[] { "db", "mongo" } // Tags para identificar o tipo de verificação
    );

// ------------ OpenTelemetry ------------
// Adiciona rastreamento (tracing) com OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            // Define o nome do serviço para rastreamento
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MinhaApiMongo"))
            // Adiciona rastreamento para requisições ASP.NET Core
            .AddAspNetCoreInstrumentation()
            // Adiciona rastreamento para chamadas HTTP
            .AddHttpClientInstrumentation()
            // Exporta os dados de rastreamento para o Jaeger
            .AddJaegerExporter(o =>
            {
                // Configura o endereço do Jaeger
                o.AgentHost = builder.Configuration["Jaeger:Host"] ?? "localhost";
                // Configura a porta do Jaeger
                o.AgentPort = int.TryParse(builder.Configuration["Jaeger:Port"], out var p) ? p : 6831;
            })
            // Exporta os dados de rastreamento para o console
            .AddConsoleExporter();
    });

// Adiciona suporte à documentação Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cria a aplicação web
var app = builder.Build();

// Configura o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    // Ativa o Swagger para documentação na fase de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisições HTTP para HTTPS
app.UseHttpsRedirection();

// Ativa a autorização (controle de acesso)
app.UseAuthorization();

// Mapeia os controladores para rotas
app.MapControllers();

// Cria o endpoint para verificar a saúde da aplicação
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Define como será a resposta do endpoint de saúde
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Inicia a aplicação
app.Run();
