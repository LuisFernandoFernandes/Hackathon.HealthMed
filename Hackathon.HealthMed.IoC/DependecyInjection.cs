﻿using FluentValidation.AspNetCore;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Mappings;
using Hackathon.HealthMed.Application.Services;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Hackathon.HealthMed.Infra.Messaging;
using Hackathon.HealthMed.Infra.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Hackathon.HealthMed.IoC;

public static class DependecyInjection
{
    public static IServiceCollection AdicionarDBContext(this IServiceCollection services, IConfiguration configuration
    )
    {
        services.AddDbContext<AppDBContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") + "Database=HealthMedDB;", sqlOptions => sqlOptions.EnableRetryOnFailure());
        });
        return services;
    }

    public static IServiceCollection AdicionarDependencias(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidationAutoValidation();


        services.AddAutoMapper(typeof(UsuarioMappingProfile));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioService, UsuarioService>();

        services.AddAutoMapper(typeof(MedicoMappingProfile));
        services.AddScoped<IMedicoRepository, MedicoRepository>();
        services.AddScoped<IMedicoService, MedicoService>();

        services.AddScoped<IPacienteRepository, PacienteRepository>();

        services.AddAutoMapper(typeof(HorarioMappingProfile));
        services.AddScoped<IHorarioRepository, HorarioRepository>();
        services.AddScoped<IHorarioService, HorarioService>();

        services.AddAutoMapper(typeof(AgendamentoMappingProfile));
        services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
        services.AddScoped<IAgendamentoService, AgendamentoService>();

        services.AddScoped<IAgendarConsultaFila, AgendarConsultaFila>();
        services.AddScoped<IRabbitMqClient, RabbitMqClient>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]))
                };
            });

        return services;
    }

}
