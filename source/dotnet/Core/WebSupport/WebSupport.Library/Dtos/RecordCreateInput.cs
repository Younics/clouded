using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebSupport.Library.Dtos;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class RecordCreateInput
{
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int Ttl { get; set; } = 600;
}
