using Hackathon.HealthMed.Agendar.Domain.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Agendar.Infra.Repository.Mapping;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        // Força o uso da tabela "Agendamentos" no schema padrão (dbo)
        builder.ToTable("Agendamentos", "dbo");

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
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.DataCriacao)
            .IsRequired()
            .HasColumnType("smalldatetime");
    }
}
