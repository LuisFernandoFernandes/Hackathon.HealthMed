using Hackathon.HealthMed.Domain.Entities;

namespace Hackathon.HealthMed.Application.Interfaces;

public interface IUsuarioService
{
    Usuario ValidarCredenciais(string login, string senha);
}
