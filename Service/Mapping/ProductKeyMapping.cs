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

        internal static ProductKey ToServiceModel(this ProductKeyEntity dataObject) => new()
        {
            Id = dataObject.Id,
            StoreName = dataObject.StoreName,
            ProductName = dataObject.ProductName,
            Key = dataObject.Key,
            Owner = dataObject.Owner,
            ConfirmationCode = dataObject.ConfirmationCode,
            Comment = dataObject.Comment,
            Status = ProductKeyStatus.FromName(dataObject.Status),
            AddedDateTime = DateTime.ParseExact(dataObject.AddedDateTime, DateTimeFormat, CultureInfo.InvariantCulture),
            UpdatedDateTime = DateTime.ParseExact(dataObject.UpdatedDateTime, DateTimeFormat, CultureInfo.InvariantCulture)
        };

        internal static ProductKeyEntity ToDataObject(this ProductKey serviceModel) => new()
        {
            Id = serviceModel.Id,
            StoreName = serviceModel.StoreName,
            ProductName = serviceModel.ProductName,
            Key = serviceModel.Key,
            Owner = serviceModel.Owner,
            ConfirmationCode = serviceModel.ConfirmationCode,
            Comment = serviceModel.Comment,
            Status = serviceModel.Status.Name,
            AddedDateTime = serviceModel.AddedDateTime.ToString(DateTimeFormat),
            UpdatedDateTime = serviceModel.UpdatedDateTime.ToString(DateTimeFormat)
        };

        internal static ProductKeyObject ToApiObject(this ProductKey serviceModel) => new()
        {
            Store = serviceModel.StoreName,
            Product = serviceModel.ProductName,
            Key = serviceModel.Key,
            Owner = serviceModel.Owner,
            Comment = serviceModel.Comment,
            Status = serviceModel.Status.Name
        };

        internal static IEnumerable<ProductKey> ToServiceModels(this IEnumerable<ProductKeyEntity> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToServiceModel());

        internal static IEnumerable<ProductKeyEntity> ToEntities(this IEnumerable<ProductKey> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToDataObject());

        internal static IEnumerable<ProductKeyObject> ToApiObjects(this IEnumerable<ProductKey> serviceModels)
            => serviceModels.Select(serviceModel => serviceModel.ToApiObject());
    }
}
