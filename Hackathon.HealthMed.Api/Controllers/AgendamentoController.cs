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

}
