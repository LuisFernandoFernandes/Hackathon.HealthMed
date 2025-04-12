using Hackathon.HealthMed.Agendar.Application.Interface;
using Hackathon.HealthMed.Agendar.Application.Service;
using Hackathon.HealthMed.Agendar.Infra.Interface;
using Hackathon.HealthMed.Agendar.Infra.Messaging;
using Hackathon.HealthMed.Agendar.Infra.Repository;
using Hackathon.HealthMed.Agendar.Infra.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hackathon.HealthMed.Agendar.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AdicionarDbContext(this IServiceCollection services, IConfiguration configurarion
    )
    {
        services.AddDbContext<AppDBContext>(options =>
        {
            options.UseSqlServer(configurarion.GetConnectionString("DefaultConnection"));
        });


        return services;
    }

    public static IServiceCollection AdicionarDependencias(this IServiceCollection services)
    {
        services.AddSingleton<RabbitMqConsumer>();
        services.AddScoped<IMessageProcessor, MensagemService>();

        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();

        return services;
    }
}
