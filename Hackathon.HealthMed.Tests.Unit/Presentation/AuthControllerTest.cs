using Hackathon.HealthMed.Api.Controllers;
using Hackathon.HealthMed.Application.DTO;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Hackathon.HealthMed.Tests.Unit.Presentation;

public class AuthControllerTest
{
    private readonly Mock<IUsuarioService> _usuarioServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _usuarioServiceMock = new Mock<IUsuarioService>();
        _configurationMock = new Mock<IConfiguration>();

        // Configuração mínima para gerar o token JWT
        _configurationMock.Setup(c => c["Jwt:SecretKey"])
                          .Returns("ThisIsASecretKey1234567890123456"); // chave com tamanho adequado
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authController = new AuthController(_usuarioServiceMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task Login_DeveRetornarOk_QuandoCredenciaisValidas()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Login = "123456",
            Senha = "senha_valida",
            TipoUsuario = eTipoUsuario.Medico
        };

        // Simula um usuário válido (adaptar conforme a implementação do seu domínio)
        var usuario = new Usuario("Usuário Teste", "usuario@teste.com", BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha), eTipoUsuario.Medico);

        _usuarioServiceMock
            .Setup(s => s.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario))
            .ReturnsAsync(usuario);

        // Act
        var result = await _authController.Login(loginDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);
        Assert.False(string.IsNullOrEmpty(loginResponse.Token));
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoCredenciaisInvalidas()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Login = "123456",
            Senha = "senha_invalida",
            TipoUsuario = eTipoUsuario.Medico
        };

        // Simula falha na validação (usuário inexistente ou credenciais erradas)
        _usuarioServiceMock
            .Setup(s => s.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario))
            .ReturnsAsync((Usuario)null);

        // Act
        var result = await _authController.Login(loginDTO);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Usuário ou senha inválidos.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoChaveJwtNaoForConfigurada()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Login = "123456",
            Senha = "senha_valida",
            TipoUsuario = eTipoUsuario.Medico
        };

        var usuario = new Usuario("Usuário Teste", "usuario@teste.com", BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha), eTipoUsuario.Medico);
        _usuarioServiceMock
            .Setup(s => s.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario))
            .ReturnsAsync(usuario);

        // Simula a falta de configuração da chave JWT
        _configurationMock.Setup(c => c["Jwt:SecretKey"]).Returns(string.Empty);

        // Recria o controller para aplicar a nova configuração
        var controllerComNovaConfiguracao = new AuthController(_usuarioServiceMock.Object, _configurationMock.Object);

        // Act
        var result = await controllerComNovaConfiguracao.Login(loginDTO);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Erro ao realizar login.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_DeveRetornarUnauthorized_QuandoOcorrerExcecaoNoServico()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Login = "123456",
            Senha = "senha_valida",
            TipoUsuario = eTipoUsuario.Medico
        };

        // Configura o serviço para lançar uma exceção durante a validação
        _usuarioServiceMock
            .Setup(s => s.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario))
            .ThrowsAsync(new Exception("Erro no serviço"));

        // Act
        var result = await _authController.Login(loginDTO);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Erro ao realizar login.", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_DeveGerarTokenComClaimsCorretas()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Login = "123456",
            Senha = "senha_valida",
            TipoUsuario = eTipoUsuario.Medico
        };

        var usuarioId = Guid.NewGuid();
        var usuario = new Usuario("Usuário Teste", "usuario@teste.com", BCrypt.Net.BCrypt.HashPassword(loginDTO.Senha), eTipoUsuario.Medico);
        typeof(Usuario)
            .GetProperty("Id")
            .SetValue(usuario, usuarioId);

        _usuarioServiceMock
            .Setup(s => s.ValidarCredenciais(loginDTO.Login, loginDTO.Senha, loginDTO.TipoUsuario))
            .ReturnsAsync(usuario);

        // Act
        var result = await _authController.Login(loginDTO);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var loginResponse = Assert.IsType<LoginResponseDTO>(okResult.Value);
        Assert.False(string.IsNullOrEmpty(loginResponse.Token));

        // Decodifica o token para verificar os claims
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(loginResponse.Token);
        var claims = jwtToken.Claims.ToList();

        var claimId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var claimRole = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        Assert.Equal(usuarioId.ToString(), claimId);
        Assert.Equal(eTipoUsuario.Medico.ToString(), claimRole);
    }
}