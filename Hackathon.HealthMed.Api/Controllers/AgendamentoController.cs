using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.HealthMed.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AgendamentoController(IAgendamentoService _agendamentoService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> AgendarConsulta([FromBody] AgendarConsultaDTO dto)
    {
        var resultado = await _agendamentoService.AgendarConsulta(dto);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpPut("confirmar")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> AceitarOuRecusarConsulta([FromBody] ConfirmarAgendamentoDTO dto)
    {
        var resultado = await _agendamentoService.ConfirmarAgendamento(dto);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpPut("cancelar-paciente")]
    [Authorize(Roles = "Paciente")]
    public async Task<IActionResult> CancelarConsultaPaciente([FromBody] CancelarAgendamentoPacienteDTO dto)
    {
        var resultado = await _agendamentoService.CancelarPorPaciente(dto);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpPut("cancelar-medico")]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> CancelarConsultaMedico([FromBody] CancelarAgendamentoMedicoDTO dto)
    {
        var resultado = await _agendamentoService.CancelarPorMedico(dto);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpGet]
    public async Task<IActionResult> ConsultarAgendamentos()
    {
        var resultado = await _agendamentoService.ConsultarAgendamentos();
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }


}
