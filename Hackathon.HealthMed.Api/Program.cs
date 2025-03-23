using Hackathon.HealthMed.Api.Filter;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.IoC;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();

builder.Services.AddControllers(options => options.Filters.Add(typeof(ModelStateValidatorFilter))).ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AdicionarDependencias(configuration);
builder.Services.AdicionarDBContext(configuration);

builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<EnumSchemaFilter>();

    // Define o esquema de segurança: Bearer
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta forma: Bearer {seu token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Configura o requisito de segurança para todos os endpoints
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

// Exemplo em Startup.cs ou Program.cs
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConfig = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"));
    return ConnectionMultiplexer.Connect(redisConfig);
});


//builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
//{
//// Filter out instrumentation of the Prometheus scraping endpoint.
//options.Filter = ctx => ctx.Request.Path != "/metrics";
//});

//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(b =>
//{
//b.AddService("PostechFase2");
//})
//    .WithTracing(b => b
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddOtlpExporter())
//    .WithMetrics(b => b
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddRuntimeInstrumentation()
//        .AddProcessInstrumentation()
//        .AddPrometheusExporter());

//builder.Services.UseHttpClientMetrics();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

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



// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapMetrics();

//app.UseMetricServer();
//app.UseHttpMetrics();
//app.UseOpenTelemetryPrometheusScrapingEndpoint();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    await context.SeedData();
}

await app.RunAsync();

public partial class Program { }
