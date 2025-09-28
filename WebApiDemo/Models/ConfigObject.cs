using Newtonsoft.Json;

namespace WebApiDemo.Models
{
    public class ConfigObject
    {
        [JsonProperty("provider_topics")]
        public required Dictionary<string, string> ProviderTopics { get; set; }
    }
}
