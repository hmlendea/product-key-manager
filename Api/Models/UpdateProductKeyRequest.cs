namespace ProductKeyManager.Api.Models
{
    public sealed class UpdateProductKeyRequest : Request
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Comment { get; set; }

        public string Status { get; set; }
    }
}
