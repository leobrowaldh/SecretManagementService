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

    public async Task<GraphApiGenericResponse<GraphApiApplicationResponse>> GetAppDataAsync()
    {
        _logger.LogInformation("Fetching authorization token...");
        var token = await _tokenService.GetAccessTokenAsync();

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
