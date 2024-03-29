using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ProductKeyManager.Api.Models;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service.Mapping
{
    static class ProductKeyMappings
    {
        const string DateTimeFormat = "yyyy.MM.ddTHH:mm:ss.ffffzzz";

        internal static ProductKey ToServiceModel(this ProductKeyEntity dataObject)
        {
            ProductKey serviceModel = new ProductKey();
            serviceModel.Id = dataObject.Id;
            serviceModel.StoreName = dataObject.StoreName;
            serviceModel.ProductName = dataObject.ProductName;
            serviceModel.Key = dataObject.Key;
            serviceModel.Owner = dataObject.Owner;
            serviceModel.ConfirmationCode = dataObject.ConfirmationCode;
            serviceModel.Comment = dataObject.Comment;
            serviceModel.Status = ProductKeyStatus.FromName(dataObject.Status);
            serviceModel.AddedDateTime = DateTime.ParseExact(dataObject.AddedDateTime, DateTimeFormat, CultureInfo.InvariantCulture);
            serviceModel.UpdatedDateTime = DateTime.ParseExact(dataObject.UpdatedDateTime, DateTimeFormat, CultureInfo.InvariantCulture);

            return serviceModel;
        }

        internal static ProductKeyEntity ToDataObject(this ProductKey serviceModel)
        {
            ProductKeyEntity dataObject = new ProductKeyEntity();
            dataObject.Id = serviceModel.Id;
            dataObject.StoreName = serviceModel.StoreName;
            dataObject.ProductName = serviceModel.ProductName;
            dataObject.Key = serviceModel.Key;
            dataObject.Owner = serviceModel.Owner;
            dataObject.ConfirmationCode = serviceModel.ConfirmationCode;
            dataObject.Comment = serviceModel.Comment;
            dataObject.Status = serviceModel.Status.Name;
            dataObject.AddedDateTime = serviceModel.AddedDateTime.ToString(DateTimeFormat);
            dataObject.UpdatedDateTime = serviceModel.UpdatedDateTime.ToString(DateTimeFormat);

            return dataObject;
        }

        internal static ProductKeyObject ToApiObject(this ProductKey serviceModel)
        {
            ProductKeyObject apiObject = new ProductKeyObject();
            apiObject.Store = serviceModel.StoreName;
            apiObject.Product = serviceModel.ProductName;
            apiObject.Key = serviceModel.Key;
            apiObject.Owner = serviceModel.Owner;
            apiObject.Comment = serviceModel.Comment;
            apiObject.Status = serviceModel.Status.Name;

            return apiObject;
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

        internal static IEnumerable<ProductKeyObject> ToApiObjects(this IEnumerable<ProductKey> serviceModels)
        {
            IEnumerable<ProductKeyObject> apiObjects = serviceModels.Select(serviceModel => serviceModel.ToApiObject());

            return apiObjects;
        }
    }
}
