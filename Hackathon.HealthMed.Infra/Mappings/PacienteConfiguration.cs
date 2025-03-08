using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Infra.Mappings;

internal class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.UsuarioId).IsRequired();
        builder.HasOne(p => p.Usuario).WithOne().HasForeignKey<Paciente>(p => p.UsuarioId);
        builder.Property(p => p.Cpf).HasMaxLength(11).IsRequired();
        builder.Property(c => c.DataCriacao).HasColumnType("smalldatetime");
    }
}
