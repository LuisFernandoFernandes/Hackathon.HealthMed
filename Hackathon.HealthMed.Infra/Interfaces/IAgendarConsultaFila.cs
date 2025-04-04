using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IAgendarConsultaFila
{
    Task AgendarAsync(Agendamento agendamento);
}
