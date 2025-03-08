namespace Hackathon.HealthMed.Domain.Entities;

/// <summary>
/// Base entity evita a repetição de código, padronizando a criação de entidades e facilita a manutenção.
/// O uso do abstract impede a instanciação da classe BaseEntity.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}

