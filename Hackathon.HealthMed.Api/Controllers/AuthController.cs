using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hackathon.HealthMed.Api.Controllers;

public class AuthController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;
    private readonly IConfiguration _configuration;

    public AuthController(IUsuarioService usuarioService, IConfiguration configuration)
    {
        _usuarioService = usuarioService;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginDTO)
    {
        try
        {
            var usuario = await _usuarioService.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario);

            if (usuario == null)
                return Unauthorized("Usuário ou senha inválidos.");

            var token = GerarToken(usuario);

            return Ok(new LoginResponseDTO { Token = token });
        }
        catch (Exception)
        {

            return Unauthorized("Erro ao realizar login.");
        }

    }

    private string GerarToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // ID do usuário
            new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()) // Tipo (Medico/Paciente)
        };

        var secretKey = _configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("Chave secreta JWT não configurada!");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
