using AutoMapper;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enums;
using Hackathon.HealthMed.Infra.Interfaces;

namespace Hackathon.HealthMed.Application.Services;

public class MedicoService(IMedicoRepository _medicoRepository, IMapper _mapper) : IMedicoService
{
    public async Task<ServiceResult<IEnumerable<MedicoDTO>>> BuscarMedicos(eEspecialidade? especialidade)
    {
        try
        {
            var medicos = await _medicoRepository.BuscarMedicos(especialidade);
            var medicosDto = _mapper.Map<IEnumerable<Medico>, IEnumerable<MedicoDTO>>(medicos);

            return new ServiceResult<IEnumerable<MedicoDTO>>(medicosDto);
        }
        catch (Exception ex)
        {
            return new ServiceResult<IEnumerable<MedicoDTO>>(ex);
        }
    }
}
