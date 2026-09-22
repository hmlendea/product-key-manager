using System;
using System.IO;
using System.Net.Http;

using NUnit.Framework;

namespace ProductKeyManager.IntegrationTests.Infrastructure
{
    public abstract class ProductKeyManagerApiTestBase
    {
        protected HttpClient Client { get; private set; } = null!;

        protected ProductKeyManagerApiFactory Factory { get; private set; } = null!;

        [SetUp]
        public void SetUp()
        {
            string storePath = Path.Combine(
                Path.GetTempPath(),
                $"product-key-manager-{Guid.NewGuid():N}.xml");
            Factory = new ProductKeyManagerApiFactory(storePath);
            Client = Factory.CreateHttpsClient();
        }

        [TearDown]
        public void TearDown()
        {
            Client.Dispose();
            Factory.Dispose();

            if (File.Exists(Factory.StorePath))
            {
                File.Delete(Factory.StorePath);
            }
        }
    }
}