namespace ProductKeyManager.Api.Models
{
    public sealed class GetProductKeyRequest : Request
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        public int Count { get; set; }
    }
}
