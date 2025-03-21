using SecretManagementService.Models.Response;
using SecretManagementService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace SecretManagementService.Services;

public class GraphApiService: IGraphApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITokenService _tokenService;
    private readonly ILogger<GraphApiService> _logger;
    public GraphApiService(
        IHttpClientFactory httpClientFactory,
        ITokenService tokenService,
        ILogger<GraphApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<List<ExpiringSecret>?> GetExpiringSecrets(int _daysUntilSecretsExpire)
    {
        _logger.LogInformation("Fetching authorization token...");
        var token = await _tokenService.GetAccessTokenAsync();

        string applicationsUri = "applications?$count=false";
        var client = _httpClientFactory.CreateClient("GraphApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(applicationsUri);
        var responseContent = await response.Content.ReadAsStringAsync();
        var graphApiApplicationsResponse = JsonSerializer.Deserialize<GraphApiGenericResponse<GraphApiApplicationResponse>>(responseContent);

        List<ExpiringSecret>? expiringSecrets = graphApiApplicationsResponse?.value
            .SelectMany(app => app.passwordCredentials
                .Where(cred => cred.endDateTime < DateTime.Now.AddDays(_daysUntilSecretsExpire))
                .Select(cred => new ExpiringSecret
                {
                    AppObjectId = app.id,
                    AppId = app.appId,
                    DisplayName = cred.displayName,
                    EndDateTime = cred.endDateTime,
                    KeyId = cred.keyId

                })
            ).ToList();

        return expiringSecrets;
    }
}
