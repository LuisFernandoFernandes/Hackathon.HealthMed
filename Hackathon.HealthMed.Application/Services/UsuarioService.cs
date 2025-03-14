using AutoMapper;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Model;
using Hackathon.HealthMed.Application.Result;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;


namespace Hackathon.HealthMed.Application.Services;

public class UsuarioService(IUsuarioRepository _usuarioRepository, IMapper _mapper) : IUsuarioService
{
    public async Task<Usuario> ValidarCredenciais(string login, string senha, eTipoUsuario tipoUsuario)
    {
        var usuario = await _usuarioRepository.ObterPorLogin(login, tipoUsuario);
        if (usuario is null || !usuario.ValidarSenha(senha))
        {
            return null;
        }
        return usuario;
    }
}
