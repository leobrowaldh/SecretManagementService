using Microsoft.AspNetCore.Authorization;
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

    //[Authorize]
    //[Function("AuthTestFunction")]
    //public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
    //ClaimsPrincipal user)
    //{
    //    if (user == null || !user.Identity?.IsAuthenticated == true)
    //        return new UnauthorizedResult();

    //    var name = user.Identity?.Name ?? "Unknown";

    //    var roles = string.Join(", ", user.FindAll(ClaimTypes.Role).Select(r => r.Value));
    //    var groups = string.Join(", ", user.FindAll("groups").Select(g => g.Value));
    //    var scopes = string.Join(", ", user.FindAll("http://schemas.microsoft.com/identity/claims/scope").Select(s => s.Value));

    //    return new OkObjectResult($"Hello {name}! Roles: {roles}. Groups: {groups}. Scopes: {scopes}");
    //}

    [Function("AuthTestFunction")]
    public IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
    ClaimsPrincipal user)
    {
        // Check if function is hit at all
        Console.WriteLine("Function invoked");

        if (user == null)
            return new OkObjectResult("User is null – function hit but no ClaimsPrincipal");

        if (!user.Identity?.IsAuthenticated == true)
            return new OkObjectResult("Function hit – user not authenticated");

        var name = user.Identity?.Name ?? "Unknown";
        var roles = string.Join(", ", user.FindAll(ClaimTypes.Role).Select(r => r.Value));
        var groups = string.Join(", ", user.FindAll("groups").Select(g => g.Value));
        var scopes = string.Join(", ", user.FindAll("http://schemas.microsoft.com/identity/claims/scope").Select(s => s.Value));

        return new OkObjectResult($"Hello {name}! Roles: {roles}. Groups: {groups}. Scopes: {scopes}");
    }
}