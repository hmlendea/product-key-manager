using System;

namespace ProductKeyManager.Service.Models
{
    public sealed class ProductKey
    {
        public string Id { get; set; }
        
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string ConfirmationCode { get; set; }

        public ProductKeyStatus Status { get; set; }

        public DateTime AddedDateTime { get; set; }

        public DateTime UpdatedDateTime { get; set; }
    }
}
