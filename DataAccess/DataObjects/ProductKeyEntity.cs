using NuciDAL.DataObjects;

namespace ProductKeyManager.DataAccess.DataObjects
{
    public sealed class ProductKeyEntity : EntityBase
    {
        public string StoreName { get; set; }

        public string ProductName { get; set; }

        public string Key { get; set; }
    }
}
