using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IAgendamentoService
{
    Task<ServiceResult<string>> AgendarConsulta(AgendarConsultaDTO dto);
    Task<ServiceResult<bool>> ConfirmarAgendamento(ConfirmarAgendamentoDTO dto);
    Task<ServiceResult<bool>> CancelarPorMedico(CancelarAgendamentoMedicoDTO dto);
    Task<ServiceResult<bool>> CancelarPorPaciente(CancelarAgendamentoPacienteDTO dto);
    Task<ServiceResult<IEnumerable<AgendamentoDTO>>> ConsultarAgendamentos();
}
