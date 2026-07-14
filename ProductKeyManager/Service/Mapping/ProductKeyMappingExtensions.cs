using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using ProductKeyManager.Api.Models;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.Service.Mapping
{
    static class ProductKeyMappingExtensions
    {
        private static string DateTimeFormat => "yyyy.MM.ddTHH:mm:ss.ffffzzz";

        internal static ProductKey ToDomainModel(this ProductKeyDataObject dataObject) => new()
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

        internal static ProductKeyDataObject ToDataObject(this ProductKey domainModel) => new()
        {
            Id = domainModel.Id,
            StoreName = domainModel.StoreName,
            ProductName = domainModel.ProductName,
            Key = domainModel.Key,
            Owner = domainModel.Owner,
            ConfirmationCode = domainModel.ConfirmationCode,
            Comment = domainModel.Comment,
            Status = domainModel.Status.Name,
            AddedDateTime = domainModel.AddedDateTime.ToString(DateTimeFormat),
            UpdatedDateTime = domainModel.UpdatedDateTime.ToString(DateTimeFormat)
        };

        internal static ProductKeyObject ToApiObject(this ProductKey domainModel) => new()
        {
            Store = domainModel.StoreName,
            Product = domainModel.ProductName,
            Key = domainModel.Key,
            Owner = domainModel.Owner,
            Comment = domainModel.Comment,
            Status = domainModel.Status.Name
        };

        internal static IEnumerable<ProductKey> ToDomainModels(
            this IEnumerable<ProductKeyDataObject> dataObjects)
            => dataObjects.Select(dataObject => dataObject.ToDomainModel());

        internal static IEnumerable<ProductKeyObject> ToApiObjects(
            this IEnumerable<ProductKey> domainModels)
            => domainModels.Select(domainModel => domainModel.ToApiObject());
    }
}
