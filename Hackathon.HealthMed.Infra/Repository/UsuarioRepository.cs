using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Context;
using Hackathon.HealthMed.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.HealthMed.Infra.Repository;

public class UsuarioRepository(AppDBContext _context) : IUsuarioRepository
{
    public async Task<Usuario> ObterPorLogin(string login, eTipoUsuario tipoUsuario)
    {
        switch (tipoUsuario)
        {
            case eTipoUsuario.Medico:
                return await ObterPorLoginMedico(login);
            case eTipoUsuario.Paciente:
                return await ObterPorLoginPaciente(login);
            default:
                return null;
        }
    }

    private async Task<Usuario> ObterPorLoginMedico(string login)
    {
        return await _context.Medicos.Where(a => a.CRM == login).Select(a => a.Usuario).FirstOrDefaultAsync();
    }

    private async Task<Usuario> ObterPorLoginPaciente(string login)
    {
        if (login.Contains("@"))
        {
            return await _context.Usuarios.Where(a => a.Email == login).FirstOrDefaultAsync();
        }
        else
        {
            return await _context.Pacientes.Where(a => a.Cpf == login).Select(a => a.Usuario).FirstOrDefaultAsync();
        }
    }
}
