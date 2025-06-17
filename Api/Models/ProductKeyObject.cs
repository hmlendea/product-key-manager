using System.Text.Json.Serialization;
using NuciSecurity.HMAC;

namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyObject
    {
        [HmacOrder(1)]
        [JsonPropertyName("store")]
        public string Store { get; set; }

        [HmacOrder(2)]
        [JsonPropertyName("product")]
        public string Product { get; set; }

        [HmacOrder(3)]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [HmacOrder(4)]
        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [HmacOrder(5)]
        [JsonPropertyName("comment")]
        public string Comment { get; set; }

        [HmacOrder(6)]
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
