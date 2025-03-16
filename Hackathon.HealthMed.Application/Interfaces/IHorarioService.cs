using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;
using Microsoft.AspNetCore.Http;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IHorarioService
{
    Task<ServiceResult<Guid>> CadastrarHorario(CadastrarHorarioDTO horario);
    Task<ServiceResult<bool>> EditarHorario(EditarHorarioDTO horarioDto);
    Task<ServiceResult<IEnumerable<HorarioDTO>>> BuscarHorarios();
}
