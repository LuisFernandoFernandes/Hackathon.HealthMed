using Hackathon.HealthMed.Api.Filter;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.IoC;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddControllers();

builder.Services.AddControllers(options => options.Filters.Add(typeof(ModelStateValidatorFilter))).ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });

builder.Services.AdicionarDependencias(configuration);
builder.Services.AdicionarDBContext(configuration);

builder.Services.AddSwaggerGen();

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
