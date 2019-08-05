namespace ProductKeyManager.Api.Models
{
    public sealed class StoreProductKeyRequest : Request
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Key { get; set; }
    }
}
