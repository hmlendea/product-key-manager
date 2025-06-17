using System.Text.Json.Serialization;

namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyObject
    {
        [JsonPropertyName("store")]
        public string Store { get; set; }

        [JsonPropertyName("product")]
        public string Product { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
