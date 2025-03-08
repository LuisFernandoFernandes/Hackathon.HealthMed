using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Infra.Mappings;

public class HorarioConfiguration : IEntityTypeConfiguration<Horario>
{
    public void Configure(EntityTypeBuilder<Horario> builder)
    {
        builder.ToTable("Horarios");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.MedicoId).IsRequired();
        builder.HasOne(h => h.Medico).WithMany().HasForeignKey(h => h.MedicoId).IsRequired();
        builder.Property(h => h.DataHorario).IsRequired();
        builder.Property(h => h.Status).IsRequired();
        builder.Property(h => h.Valor).IsRequired().HasColumnType("decimal(10,2)");
        builder.Property(c => c.DataCriacao).HasColumnType("smalldatetime");
    }
}
