using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NuciAPI.Requests;
using NuciSecurity.HMAC;

namespace ProductKeyManager.Api.Models
{
    public sealed class GetProductKeyRequest : Request
    {
        [HmacOrder(1)]
        [JsonPropertyName("store")]
        public string StoreName { get; set; }

        [HmacOrder(2)]
        [JsonPropertyName("product")]
        public string ProductName { get; set; }

        [HmacOrder(3)]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [HmacOrder(4)]
        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [HmacOrder(5)]
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [HmacOrder(6)]
        [JsonPropertyName("count")]
        [Range(1, 1000, ErrorMessage = "Count must be between 1 and 1000.")]
        public int Count { get; set; } = 1;
    }
}
