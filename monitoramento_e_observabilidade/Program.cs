// Importa as bibliotecas necess�rias para monitoramento, verifica��o de sa�de, MongoDB e rastreamento
using HealthChecks.MongoDb;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Cria o construtor da aplica��o web
var builder = WebApplication.CreateBuilder(args);

// Adiciona o servi�o de controladores (Controllers) � aplica��o
builder.Services.AddControllers();

// ------------ Health Checks ------------
// Adiciona verifica��es de sa�de � aplica��o
builder.Services.AddHealthChecks()
    // Verifica��o simples para saber se a API est� funcionando
    .AddCheck("self", () => HealthCheckResult.Healthy("API OK"))
    // Verifica��o de sa�de do MongoDB
    .AddMongoDb(
        sp =>
        {
            // Obt�m a string de conex�o do MongoDB das configura��es
            var cs = builder.Configuration["MongoDb:ConnectionString"];
            return new MongoClient(cs);
        },
        name: "mongodb", // Nome da verifica��o
        timeout: TimeSpan.FromSeconds(5), // Tempo limite de 5 segundos
        tags: new[] { "db", "mongo" } // Tags para identificar o tipo de verifica��o
    );

// ------------ OpenTelemetry ------------
// Adiciona rastreamento (tracing) com OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            // Define o nome do servi�o para rastreamento
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MinhaApiMongo"))
            // Adiciona rastreamento para requisi��es ASP.NET Core
            .AddAspNetCoreInstrumentation()
            // Adiciona rastreamento para chamadas HTTP
            .AddHttpClientInstrumentation()
            // Exporta os dados de rastreamento para o Jaeger
            .AddJaegerExporter(o =>
            {
                // Configura o endere�o do Jaeger
                o.AgentHost = builder.Configuration["Jaeger:Host"] ?? "localhost";
                // Configura a porta do Jaeger
                o.AgentPort = int.TryParse(builder.Configuration["Jaeger:Port"], out var p) ? p : 6831;
            })
            // Exporta os dados de rastreamento para o console
            .AddConsoleExporter();
    });

// Adiciona suporte � documenta��o Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cria a aplica��o web
var app = builder.Build();

// Configura o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    // Ativa o Swagger para documenta��o na fase de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona requisi��es HTTP para HTTPS
app.UseHttpsRedirection();

// Ativa a autoriza��o (controle de acesso)
app.UseAuthorization();

// Mapeia os controladores para rotas
app.MapControllers();

// Cria o endpoint para verificar a sa�de da aplica��o
app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Define como ser� a resposta do endpoint de sa�de
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Inicia a aplica��o
app.Run();
