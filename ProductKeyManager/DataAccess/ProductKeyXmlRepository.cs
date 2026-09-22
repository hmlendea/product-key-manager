using System.Linq;

using NuciDAL.Repositories;

using ProductKeyManager.DataAccess.DataObjects;

namespace ProductKeyManager.DataAccess
{
    public sealed class ProductKeyXmlRepository(string fileName)
        : XmlRepository<ProductKeyDataObject>(fileName)
    {
        protected override void PerformFileSave()
            => XmlFile.SaveEntities(GetAll().ToList());
    }
}