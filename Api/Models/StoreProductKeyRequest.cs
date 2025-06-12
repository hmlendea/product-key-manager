using System.Text.Json.Serialization;
using NuciAPI.Requests;

namespace ProductKeyManager.Api.Models
{
    public sealed class StoreProductKeyRequest : Request
    {
        [JsonPropertyName("store")]
        public string StoreName { get; set; }

        [JsonPropertyName("product")]
        public string ProductName { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Comment { get; set; }

        public string Status { get; set; }
    }
}
