using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Tests.Integration.Fixture;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



namespace Hackathon.HealthMed.Tests.Integration;

[Collection(nameof(ContextDbCollection))]
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public string conectionString { get; set; } = "";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDBContext>));

            services.Remove(dbContextDescriptor!);

            services.AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlServer(conectionString);
            });
        });

        builder.UseEnvironment("Development");
    }
}
