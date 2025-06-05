using System.ComponentModel.DataAnnotations;

namespace ProductKeyManager.Api.Models
{
    public class ProductKeyQuery
    {
        public string Store { get; set; }

        public string Product { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        [Range(1, 1000)]
        public int? Count { get; set; }

        public string Hmac { get; set; }

        public string Comment { get; set; }
    }
}
