using AutoMapper;
using BCrypt.Net;
using Hackathon.HealthMed.Application.Interfaces;
using Hackathon.HealthMed.Application.Services;
using Hackathon.HealthMed.Domain.Entities;
using Hackathon.HealthMed.Domain.Enum;
using Hackathon.HealthMed.Infra.Interfaces;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Hackathon.HealthMed.Tests.Unit.Application.Services
{
    public class UsuarioServiceTest
    {
        private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public UsuarioServiceTest()
        {
            _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
            var mapperMock = new Mock<IMapper>();
            _mapper = mapperMock.Object;

            _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task ValidarCredenciais_ReturnsUser_WhenCredentialsAreValid()
        {
            // Arrange
            var login = "123456";
            var senha = "senha123";
            var tipoUsuario = eTipoUsuario.Medico;
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
            var usuario = new Usuario("Fulano", "fulano@test.com", senhaHash, tipoUsuario);

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorLogin(login, tipoUsuario))
                .ReturnsAsync(usuario);

            // Act
            var result = await _usuarioService.ValidarCredenciais(login, senha, tipoUsuario);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(usuario, result);
        }

        [Fact]
        public async Task ValidarCredenciais_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var login = "123456";
            var senha = "senha123";
            var tipoUsuario = eTipoUsuario.Medico;

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorLogin(login, tipoUsuario))
                .ReturnsAsync((Usuario)null);

            // Act
            var result = await _usuarioService.ValidarCredenciais(login, senha, tipoUsuario);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidarCredenciais_ReturnsNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var login = "123456";
            var senha = "senha123";
            var wrongSenha = "senhaErrada";
            var tipoUsuario = eTipoUsuario.Medico;
            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
            var usuario = new Usuario("Fulano", "fulano@test.com", senhaHash, tipoUsuario);

            _usuarioRepositoryMock
                .Setup(repo => repo.ObterPorLogin(login, tipoUsuario))
                .ReturnsAsync(usuario);

            // Act
            var result = await _usuarioService.ValidarCredenciais(login, wrongSenha, tipoUsuario);

            // Assert
            Assert.Null(result);
        }
    }
}
