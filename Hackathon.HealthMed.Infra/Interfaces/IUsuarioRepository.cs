using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;

namespace Hackathon.HealthMed.Infra.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> ObterPorLogin(string login, eTipoUsuario tipoUsuario);
}
