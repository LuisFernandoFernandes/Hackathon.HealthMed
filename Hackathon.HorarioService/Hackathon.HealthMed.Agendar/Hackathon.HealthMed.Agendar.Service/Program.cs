using Hackathon.HealthMed.Agendar.IoC;
using Hackathon.HealthMed.Agendar.Service;
using Prometheus;
using Serilog;


var builder = Host
    .CreateDefaultBuilder(args)

    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            app.UseRouting();
            app.UseMetricServer();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
            });
        });

        webBuilder.UseUrls("http://+:5001");
    })
    .ConfigureServices((hostContext, services) =>
    {

        services.AddHostedService<Worker>();
        services.AdicionarDependencias();
        services.AdicionarDbContext(hostContext.Configuration);
    })
    .UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console());



var host = builder.Build();
await host.StartAsync();
await Task.Delay(Timeout.Infinite);