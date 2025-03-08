using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Infra.Mappings;

public class MedicoConfiguration : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        builder.ToTable("Medicos");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.UsuarioId).IsRequired();
        builder.HasOne(m => m.Usuario).WithOne().HasForeignKey<Medico>(m => m.UsuarioId);
        builder.Property(m => m.CRM).HasMaxLength(10).IsRequired();
        builder.Property(m => m.Especialidade).IsRequired();
        builder.Property(c => c.DataCriacao).HasColumnType("smalldatetime");
    }
}
