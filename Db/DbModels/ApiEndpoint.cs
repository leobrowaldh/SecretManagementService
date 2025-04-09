using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public string BaseUrl { get; set; }  // API base URL
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

    public Secret? Secret { get; set; }
}
