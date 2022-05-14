namespace ProductKeyManager.Api.Models
{
    public sealed class ProductKeyObject
    {
        public string Store { get; set; }

        public string Product { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Comment { get; set; }

        public string Status { get; set; }
    }
}
