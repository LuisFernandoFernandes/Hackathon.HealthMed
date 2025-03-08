using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Infra.Mappings;
public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamentos");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.PacienteId).IsRequired();
        builder.HasOne(a => a.Paciente).WithMany().HasForeignKey(a => a.PacienteId).IsRequired();
        builder.Property(a => a.HorarioId).IsRequired();
        builder.HasOne(a => a.Horario).WithOne().HasForeignKey<Agendamento>(a => a.HorarioId).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.JustificativaCancelamento).HasMaxLength(255).IsRequired(false);
        builder.Property(c => c.DataCriacao).HasColumnType("smalldatetime");
    }
}
