using NuciDAL.DataObjects;

namespace ProductKeyManager.DataAccess.DataObjects
{
    public sealed class ProductKeyEntity : EntityBase
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Key { get; set; }

        public string Owner { get; set; }

        public string Status { get; set; }

        public string AddedDateTime { get; set; }

        public string UpdatedDateTime { get; set; }
    }
}
