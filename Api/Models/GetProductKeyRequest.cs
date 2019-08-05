namespace ProductKeyManager.Api.Models
{
    public sealed class GetProductKeyRequest : Request
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }
    }
}
