using SecretManagementService.Models.Response;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace SecretManagementService.Services;

public class GraphApiService: IGraphApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAzureTokenService _tokenService;
    private readonly ILogger<GraphApiService> _logger;
    public GraphApiService(
        IHttpClientFactory httpClientFactory,
        IAzureTokenService tokenService,
        ILogger<GraphApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<GraphApiGenericResponse<GraphApiApplicationResponse>> GetAppDataAsync()
    {
        _logger.LogInformation("Fetching authorization token...");
        var token = await _tokenService.GetGraphApiAccessTokenAsync();

        string applicationsUri = "applications?$count=false";
        var client = _httpClientFactory.CreateClient("GraphApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        try
        {
            var response = await client.GetAsync(applicationsUri);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var graphApiApplicationsResponse = JsonSerializer.Deserialize<GraphApiGenericResponse<GraphApiApplicationResponse>>(responseContent);
            if (graphApiApplicationsResponse == null)
            {
                _logger.LogError("Deserialization returned null. Response content: {ResponseContent}", responseContent);
                throw new InvalidOperationException("Failed to deserialize Graph API response.");
            }

            return graphApiApplicationsResponse;
        }
        catch (HttpRequestException httpRequestException)
        {
            _logger.LogError(httpRequestException, "Error occurred while making HTTP request to Graph API.");
            throw;
        }
    }

}
