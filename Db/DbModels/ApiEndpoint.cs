using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.DbModels;

public enum EnHttpMethod
{
    Get,
    Post,
    Put,
    Patch,
    Delete
}

[Table("ApiEndpoints", Schema = "suprusr")]
public class ApiEndpoint
{
    public Guid ApiEndpointId { get; set; }
    [Url]
    public required string BaseUrl { get; set; }  // API base URL
    public string QueryParametersJson { get; set; } = "{}";
    public string HeadersJson { get; set; } = "{}";
    public EnHttpMethod HttpMethod { get; set; } = (EnHttpMethod)1; // HTTP method (default to POST)
    public string StrHttpMethod
    {
        get => HttpMethod.ToString();
        set {}
    }
    public string? BodyTemplate { get; init; } // Optional JSON/XML template for body payload
    public bool Seeded { get; set; }

    public Guid SubscriberId { get; set; }
    public Subscriber Subscriber { get; set; }
    public bool IsDeleted { get; set; }
}
