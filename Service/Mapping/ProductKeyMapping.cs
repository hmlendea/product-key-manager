using System.Collections.Generic;
using System.Linq;

using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service.Mapping
{
    static class ProductKeyMappings
    {
        internal static ProductKey ToServiceModel(this ProductKeyEntity dataObject)
        {
            ProductKey serviceModel = new ProductKey();
            serviceModel.Id = dataObject.Id;
            serviceModel.StoreName = dataObject.StoreName;
            serviceModel.ProductName = dataObject.ProductName;
            serviceModel.Key = dataObject.Key;

            return serviceModel;
        }

        internal static ProductKeyEntity ToDataObject(this ProductKey serviceModel)
        {
            ProductKeyEntity dataObject = new ProductKeyEntity();
            dataObject.Id = serviceModel.Id;
            dataObject.StoreName = serviceModel.StoreName;
            dataObject.ProductName = serviceModel.ProductName;
            dataObject.Key = serviceModel.Key;

            return dataObject;
        }

        internal static IEnumerable<ProductKey> ToServiceModels(this IEnumerable<ProductKeyEntity> dataObjects)
        {
            IEnumerable<ProductKey> serviceModels = dataObjects.Select(dataObject => dataObject.ToServiceModel());

            return serviceModels;
        }

        internal static IEnumerable<ProductKeyEntity> ToEntities(this IEnumerable<ProductKey> serviceModels)
        {
            IEnumerable<ProductKeyEntity> dataObjects = serviceModels.Select(serviceModel => serviceModel.ToDataObject());

            return dataObjects;
        }
    }
}
