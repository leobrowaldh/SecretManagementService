using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace SMSFunctionApp.Functions;

public class ApiTestFunction
{
    private readonly ILogger<ApiTestFunction> _logger;

    public ApiTestFunction(ILogger<ApiTestFunction> logger)
    {
        _logger = logger;
    }

    [Function("ApiTestFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string name = req.Query["name"];

        if (string.IsNullOrEmpty(name))
        {
            return new BadRequestObjectResult("Please pass a name on the query string");
        }

        return new OkObjectResult($"Hi, {name}!");
    }
}
