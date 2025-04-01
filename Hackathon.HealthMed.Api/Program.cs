using Hackathon.HealthMed.Api.Filter;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.IoC;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using Prometheus;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configuração dos controllers e filtros
builder.Services.AddControllers();
builder.Services.AddControllers(options =>
    options.Filters.Add(typeof(ModelStateValidatorFilter)))
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

// Injeção de dependências e configuração do DB
builder.Services.AdicionarDependencias(configuration);
builder.Services.AdicionarDBContext(configuration);

// Configuração do Swagger com JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Configuração do Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
    return ConnectionMultiplexer.Connect(redisConfig);
});

// Filtrar a instrumentação para não interferir no endpoint /metrics
builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
{
    options.Filter = ctx => ctx.Request.Path != "/metrics";
});

// Configuração do OpenTelemetry (Tracing e Metrics)
// Aqui são adicionadas as instrumentações para ASP.NET Core, HTTP, runtime, process e o exporter do Prometheus
builder.Services.AddOpenTelemetry()
    .ConfigureResource(b => b.AddService("HealthMed"))
    .WithTracing(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(b => b
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddProcessInstrumentation()
        .AddPrometheusExporter());

builder.Services.UseHttpClientMetrics();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configurações para ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

// Configuração do pipeline HTTP
app.UseAuthorization();
app.MapControllers();
//app.MapMetrics();
//app.UseMetricServer();
//app.UseHttpMetrics();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Política de retry para a migração e seed do banco de dados
var retryPolicy = Policy
    .Handle<SqlException>()
    .WaitAndRetryAsync(10, i => TimeSpan.FromSeconds(5),
        (exception, timeSpan, retryCount, context) =>
        {
            Console.WriteLine($"⏳ Tentativa {retryCount}: SQL Server ainda não está pronto.");
        });

await retryPolicy.ExecuteAsync(async () =>
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    await dbContext.Database.MigrateAsync();
    await dbContext.SeedData();
});

await app.RunAsync();

public partial class Program { }
