using System;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using NuciDAL.Repositories;

using NUnit.Framework;

using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.IntegrationTests.Infrastructure;

namespace ProductKeyManager.IntegrationTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class ProductKeyXmlPersistenceIntegrationTests : ProductKeyManagerApiTestBase
    {
        [Test]
        public void GivenAProductKeyDataObject_WhenSavingThroughTheRegisteredRepository_ThenTheXmlFileCanBeReadBack()
        {
            IFileRepository<ProductKeyDataObject> repository =
                Factory.Services.GetRequiredService<IFileRepository<ProductKeyDataObject>>();
            ProductKeyDataObject productKey = new()
            {
                Id = "integration-id",
                StoreName = "Store",
                ProductName = "Product",
                Key = "KEY-001",
                Status = "Vacant",
                AddedDateTime = DateTime.UtcNow.ToString("O"),
                UpdatedDateTime = DateTime.UtcNow.ToString("O")
            };

            repository.Add(productKey);
            repository.SaveChanges();

            Assert.That(File.Exists(Factory.StorePath));
            Assert.That(repository.Get("integration-id").Key, Is.EqualTo("KEY-001"));
        }
    }
}