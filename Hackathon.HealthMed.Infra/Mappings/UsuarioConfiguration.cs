using Hackathon.HealthMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hackathon.HealthMed.Infra.Mappings;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Nome).HasMaxLength(100).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150).IsRequired(false); // Opcional para médicos
        builder.Property(u => u.SenhaHash).HasMaxLength(255).IsRequired();
        builder.Property(u => u.TipoUsuario).IsRequired();
        builder.Property(u => u.Ativo).IsRequired();
    }
}
