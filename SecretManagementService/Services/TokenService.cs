using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;

namespace SecretManagementService.Services;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenService> _logger;
    private readonly string _tenantId;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public TokenService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;

        try
        {
            _clientId = configuration["CLIENT_ID"]
                ?? throw new ArgumentNullException("CLIENT_ID not found in configuration");
            _tenantId = configuration["TENANT_ID"]
                ?? throw new ArgumentNullException("TENANT_ID not found in configuration");
            _clientSecret = configuration["client-secret"]
                ?? throw new ArgumentNullException("client-secret not found in configuration");
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the TokenService.");
            throw;
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tokenUri = $"{_tenantId}/oauth2/v2.0/token";

        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", _clientId),

            new KeyValuePair<string, string>("client_secret", _clientSecret),

            new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });

        var httpClient = _httpClientFactory.CreateClient(name: "AzureAuth");

        try
        {
            var response = await httpClient.PostAsync(tokenUri, body);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);
            return data.GetProperty("access_token").GetString()
                ?? throw new NullReferenceException("access_token not found in response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while getting the access token.");
            throw;
        }
    }
}

