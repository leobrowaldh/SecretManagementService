using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SMSFunctionApp.Helpers;
using System.IdentityModel.Tokens.Jwt;
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
    public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("Function invoked");

        var authHeader = req.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            return new UnauthorizedResult();

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://login.microsoftonline.com/d87548be-2c3c-454d-8214-8941643fc99f/v2.0", 
            ValidAudience = "api://4e72630e-e7a9-444f-8287-c7b704149746",
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKeys = await AzureAuthHelper.GetSigningKeysAsync("d87548be-2c3c-454d-8214-8941643fc99f")
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

            var name = principal.Identity?.Name ?? "Unknown";
            var roles = string.Join(", ", principal.FindAll(ClaimTypes.Role).Select(r => r.Value));
            var scopes = string.Join(", ", principal.FindAll("http://schemas.microsoft.com/identity/claims/scope").Select(s => s.Value));

            return new OkObjectResult($"Hello {name}! Roles: {roles}. Scopes: {scopes}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed");
            return new UnauthorizedResult();
        }
    }

}