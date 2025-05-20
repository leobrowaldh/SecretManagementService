using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SMSFunctionApp.AzureHelpers;
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
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var user = ClaimsPrincipalParser.Parse(req);

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return new UnauthorizedResult();
        }

        var name = user.Identity.Name ?? user.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        var roles = string.Join(", ", user.FindAll(ClaimTypes.Role).Select(r => r.Value));

        return new OkObjectResult($"Hello {name}! Your roles: {roles}");
    }
}