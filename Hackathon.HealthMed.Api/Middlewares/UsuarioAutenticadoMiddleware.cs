using System.Security.Claims;

namespace Hackathon.HealthMed.Api.Middlewares;

public class UsuarioAutenticadoMiddleware
{
    private readonly RequestDelegate _next;

    public UsuarioAutenticadoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userRole))
            {
                context.Items["UserId"] = userId;
                context.Items["TipoUsuario"] = userRole;
            }
        }

        await _next(context);
    }
}