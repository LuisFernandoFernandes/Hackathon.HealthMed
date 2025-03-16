using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.HealthMed.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HorarioController(IHorarioService _horarioService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> CadastrarHorario([FromBody] CadastrarHorarioDTO horario)
    {
        var resultado = await _horarioService.CadastrarHorario(horario);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpPut]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> EditarHorario([FromBody] EditarHorarioDTO horario)
    {
        var resultado = await _horarioService.EditarHorario(horario);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

    [HttpGet]
    [Authorize(Roles = "Medico")]
    public async Task<IActionResult> GetHorarios()
    {
        var resultado = await _horarioService.BuscarHorarios();
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }
}
