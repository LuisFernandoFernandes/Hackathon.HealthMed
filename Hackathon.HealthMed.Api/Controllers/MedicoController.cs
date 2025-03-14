using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.HealthMed.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MedicoController(IMedicoService medicoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMedicos([FromQuery] eEspecialidade? especialidade)
    {
        var resultado = await medicoService.BuscarMedicos(especialidade);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }
}
