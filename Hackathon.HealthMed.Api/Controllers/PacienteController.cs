using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Hackathon.HealthMed.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PacienteController(IPacienteService pacienteService) : ControllerBase
{


}
