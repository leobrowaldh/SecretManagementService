using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSFunctionApp.Models.DTOs;
public class ApiInfo
{
    public required Guid SecretId { get; init; } // Identifier for client secret in Key Vault
    public required string BaseUrl { get; init; }  // API base URL
    public Dictionary<string, string> QueryParameters { get; init; } = new(); // Query parameters
    public Dictionary<string, string> Headers { get; init; } = new(); // HTTP headers
    public HttpMethod Method { get; init; } = HttpMethod.Post; // HTTP method (default to POST)
    public string? BodyTemplate { get; init; } // Optional JSON/XML template for body payload
}
