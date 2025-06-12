using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NuciAPI.Requests;

namespace ProductKeyManager.Api.Models
{
    public sealed class GetProductKeyRequest : Request
    {
        [JsonPropertyName("store")]
        public string StoreName { get; set; }

        [JsonPropertyName("product")]
        public string ProductName { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        [Range(1, 1000, ErrorMessage = "Count must be between 1 and 1000.")]
        public int Count { get; set; } = 1;
    }
}
