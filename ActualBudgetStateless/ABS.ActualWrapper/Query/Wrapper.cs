using System.Text.Json.Serialization;

namespace ABS.ActualWrapper.Query;

public class Wrapper
{
    [JsonPropertyName("ActualQLquery")]
    public AqlQuery AqlQuery { get; set; }
}