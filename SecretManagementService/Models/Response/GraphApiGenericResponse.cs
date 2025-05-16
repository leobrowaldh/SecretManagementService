using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SecretManagementService.Models.Response;

public record GraphApiGenericResponse<T>
{
    [JsonPropertyName("@odata.context")]
    public string odatacontext { get; set; } = "";
    public T[] value { get; set; } = Array.Empty<T>();
}
