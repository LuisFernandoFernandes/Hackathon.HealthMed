using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IUsuarioService
{
    Task<Usuario> ValidarCredenciais(string login, string senha, eTipoUsuario tipoUsuario);
}
