using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Hackathon.HealthMed.Tests.Integration;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Lê o cabeçalho "X-Test-Roles" para definir as roles; se não existir, assume "Paciente"
        string rolesHeader = Request.Headers.ContainsKey("X-Test-Roles")
            ? Request.Headers["X-Test-Roles"].ToString()
            : "Paciente";

        // Lê o cabeçalho "X-Test-UserId" para definir o ID do usuário; se não existir, usa um valor padrão
        string userId = Request.Headers.ContainsKey("X-Test-UserId")
            ? Request.Headers["X-Test-UserId"].ToString()
            : "default-user-id";

        // Permite múltiplas roles separadas por vírgula
        var roles = rolesHeader.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, "TestUser"),
        new Claim(ClaimTypes.NameIdentifier, userId)
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Trim()));
        }

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

}