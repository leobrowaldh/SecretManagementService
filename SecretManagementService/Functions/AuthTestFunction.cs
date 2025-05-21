using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace SMSFunctionApp.Functions;

public class AuthTestFunction
{
    private readonly ILogger<AuthTestFunction> _logger;

    public AuthTestFunction(ILogger<AuthTestFunction> logger)
    {
        _logger = logger;
    }

    [Function("AuthTestFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
    FunctionContext executionContext)
    {
        var principal = req.HttpContext.User;

        if (principal == null || !principal.Identity?.IsAuthenticated == true)
        {
            return new UnauthorizedResult();
        }

        var name = principal.Identity?.Name ?? "Unknown";
        var roles = string.Join(", ", principal.FindAll(ClaimTypes.Role).Select(r => r.Value));

        return new OkObjectResult($"Hello {name}! Your roles: {roles}");
    }
}