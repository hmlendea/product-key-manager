using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

using ProductKeyManager.Api.Models;
using ProductKeyManager.IntegrationTests.Assertions;
using ProductKeyManager.IntegrationTests.Infrastructure;

namespace ProductKeyManager.IntegrationTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class ProductKeysApiPersistenceIntegrationTests : ProductKeyManagerApiTestBase
    {
        [Test]
        public async Task GivenAProductKeyAddedByOneHost_WhenAnotherHostUsesTheSameStore_ThenTheKeyIsAvailable()
        {
            string storePath = Factory.StorePath;
            await AddProductKeyAsync("KEY-RESTART");

            Client.Dispose();
            Factory.Dispose();

            using ProductKeyManagerApiFactory restartedFactory = new(storePath);
            using HttpClient restartedClient = restartedFactory.CreateHttpsClient();
            using HttpResponseMessage response = await restartedClient.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest
                {
                    Key = "KEY-RESTART",
                    Count = 1
                }));
            using JsonDocument result = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                response,
                1);

            Assert.That(
                result.RootElement.GetProperty("products")[0].GetProperty("key").GetString(),
                Is.EqualTo("KEY-RESTART"));
            Assert.That(File.Exists(storePath));
        }

        private async Task AddProductKeyAsync(string key)
        {
            using HttpResponseMessage response = await Client.SendAsync(
                ProductKeyRequestFactory.CreateAddRequest(new AddProductKeyRequest
                {
                    StoreName = "Store",
                    ProductName = "Product",
                    Key = key,
                    Owner = "Owner",
                    Comment = "Comment",
                    Status = "Vacant"
                }));

            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(response);
        }
    }
}