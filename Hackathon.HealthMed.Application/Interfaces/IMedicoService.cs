using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Enums;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IMedicoService
{
    Task<ServiceResult<IEnumerable<MedicoDTO>>> BuscarMedicos(eEspecialidade? especialidade);
}
