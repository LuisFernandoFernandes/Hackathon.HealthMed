using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IAgendamentoService
{
    Task<ServiceResult<Guid>> AgendarConsulta(AgendarConsultaDTO dto);
}
