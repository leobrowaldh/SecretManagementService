using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SecretManagementService.Services;

namespace SecretManagementService.Functions;

public class TestFunction
{
    private readonly ILogger<TestFunction> _logger;
    private readonly IAzureTokenService _tokenService;

    public TestFunction(ILogger<TestFunction> logger, IAzureTokenService tokenService)
    {
        _logger = logger;
        _tokenService = tokenService;
    }

    [Function("TestFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var token = await _tokenService.GetAccessTokenAsync();
        _logger.LogInformation($"Generated Token: {token}");

        return new OkObjectResult("Welcome to Azure Functions!");
    }
}
