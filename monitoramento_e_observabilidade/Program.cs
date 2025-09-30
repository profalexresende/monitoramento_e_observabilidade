using HealthChecks.MongoDb;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ------------ Health Checks ------------
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("API OK"))
    .AddMongoDb(
        sp =>
        {
            var cs = builder.Configuration["MongoDb:ConnectionString"];
            return new MongoClient(cs);
        },
        name: "mongodb",
        timeout: TimeSpan.FromSeconds(5),
        tags: new[] { "db", "mongo" }
    );

// ------------ OpenTelemetry ------------
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MinhaApiMongo"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddJaegerExporter(o =>
            {
                o.AgentHost = builder.Configuration["Jaeger:Host"] ?? "localhost";
                o.AgentPort = int.TryParse(builder.Configuration["Jaeger:Port"], out var p) ? p : 6831;
            })
            .AddConsoleExporter();
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Endpoint de health
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
