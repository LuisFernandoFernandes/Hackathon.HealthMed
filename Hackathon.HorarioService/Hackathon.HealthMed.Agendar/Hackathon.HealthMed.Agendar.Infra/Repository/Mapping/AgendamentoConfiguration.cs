using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Agendar.Infra.Repository.Mapping;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasColumnType("uniqueidentifier")
            .ValueGeneratedNever();

        builder.Property(a => a.PacienteId)
            .IsRequired();

        builder.Property(a => a.HorarioId)
            .IsRequired();

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.JustificativaCancelamento)
            .HasMaxLength(500);

        builder.Property(a => a.DataCriacao)
            .IsRequired()
            .HasColumnType("datetime2");
    }
}
