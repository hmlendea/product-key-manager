using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public sealed class ProductKeysApiIntegrationTests : ProductKeyManagerApiTestBase
    {
        [TestCase("Unknown")]
        [TestCase("Used")]
        [TestCase("Vacant")]
        [TestCase("Invalid")]
        [TestCase("AlreadyOwned")]
        [TestCase("RequiresBaseProduct")]
        [TestCase("RegionLocked")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("NotARecognisedStatus")]
        public async Task GivenAProductKeyRequest_WhenAddingAndRetrievingIt_ThenAllWireValuesRoundTrip(
            string? status)
        {
            AddProductKeyRequest addRequest = CreateAddRequest(
                "Store with spaces",
                "Product/Edition",
                $"KEY-{status ?? "NULL"}-001",
                "Owner_001",
                "Comment with punctuation: !?;",
                status);

            using HttpResponseMessage addResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateAddRequest(addRequest));

            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(addResponse);

            using HttpResponseMessage getResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest
                {
                    Key = addRequest.Key,
                    Count = 1
                }));
            using JsonDocument response = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                getResponse,
                1);
            JsonElement product = response.RootElement.GetProperty("products")[0];

            Assert.Multiple(() =>
            {
                Assert.That(product.GetProperty("store").GetString(), Is.EqualTo(addRequest.StoreName));
                Assert.That(product.GetProperty("product").GetString(), Is.EqualTo(addRequest.ProductName));
                Assert.That(product.GetProperty("key").GetString(), Is.EqualTo(addRequest.Key));
                Assert.That(product.GetProperty("owner").GetString(), Is.EqualTo(addRequest.Owner));
                Assert.That(product.GetProperty("comment").GetString(), Is.EqualTo(addRequest.Comment));
                Assert.That(product.GetProperty("status").GetString(), Is.EqualTo(ExpectedStatus(status)));
            });
        }

        [Test]
        public async Task GivenMultipleProductKeys_WhenRetrievingWithCountBoundaries_ThenTheResultIsLimitedAndOrdered()
        {
            await AddProductKeyAsync("Store", "Product", "KEY-C", "Owner", "Comment", "Vacant");
            await AddProductKeyAsync("Store", "Product", "KEY-A", "Owner", "Comment", "Vacant");
            await AddProductKeyAsync("Store", "Product", "KEY-B", "Owner", "Comment", "Vacant");

            using HttpResponseMessage firstResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest { Count = 1 }));
            using JsonDocument firstResult = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                firstResponse,
                1);

            using HttpResponseMessage allResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest { Count = 1000 }));
            using JsonDocument allResult = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                allResponse,
                3);
            IEnumerable<string?> keys = allResult.RootElement
                .GetProperty("products")
                .EnumerateArray()
                .Select(product => product.GetProperty("key").GetString());

            Assert.Multiple(() =>
            {
                Assert.That(firstResult.RootElement.GetProperty("products").GetArrayLength(), Is.EqualTo(1));
                Assert.That(keys, Is.EqualTo(["KEY-A", "KEY-B", "KEY-C"]));
            });
        }

        [TestCase("Store", "Store", 1)]
        [TestCase("Store", "^Sto", 1)]
        [TestCase("Store", "ore$", 2)]
        [TestCase("Product", "Product", 1)]
        [TestCase("Key", "KEY-002", 1)]
        [TestCase("Owner", "Owner-002", 1)]
        [TestCase("Status", "Used", 1)]
        [TestCase("Store", "NoMatch", 0)]
        public async Task GivenDifferentProductKeyAttributes_WhenFiltering_ThenOnlyMatchingRecordsAreReturned(
            string filterName,
            string filterValue,
            int expectedCount)
        {
            await AddProductKeyAsync("Store", "Product", "KEY-001", "Owner-001", "Comment", "Vacant");
            await AddProductKeyAsync("OtherStore", "OtherProduct", "KEY-002", "Owner-002", "Other", "Used");

            GetProductKeyRequest request = CreateGetRequestWithFilter(filterName, filterValue);

            using HttpResponseMessage response = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(request));

            if (expectedCount == 0)
            {
                await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.InternalServerError);

                return;
            }

            using JsonDocument result = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                response,
                expectedCount);

            IEnumerable<string?> keys = result.RootElement
                .GetProperty("products")
                .EnumerateArray()
                .Select(product => product.GetProperty("key").GetString());

            if (expectedCount == 2)
            {
                Assert.That(keys, Is.EquivalentTo(["KEY-001", "KEY-002"]));

                return;
            }

            Assert.That(keys.Single(), Is.EqualTo(ExpectedKeyForFilter(filterName)));
        }

        [Test]
        public async Task GivenAnExistingProductKey_WhenUpdatingAllMutableFields_ThenTheUpdatedValuesArePersisted()
        {
            await AddProductKeyAsync("OriginalStore", "OriginalProduct", "KEY-UPDATE", "OriginalOwner", "OriginalComment", "Vacant");

            UpdateProductKeyRequest updateRequest = new()
            {
                StoreName = "UpdatedStore",
                ProductName = "UpdatedProduct",
                Key = "KEY-UPDATE",
                Owner = "UpdatedOwner",
                Comment = "UpdatedComment",
                Status = "Used"
            };
            using HttpResponseMessage updateResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateUpdateRequest(updateRequest));

            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(updateResponse);

            using HttpResponseMessage getResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest
                {
                    Key = updateRequest.Key,
                    Count = 1
                }));
            using JsonDocument result = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                getResponse,
                1);
            JsonElement product = result.RootElement.GetProperty("products")[0];

            Assert.Multiple(() =>
            {
                Assert.That(product.GetProperty("store").GetString(), Is.EqualTo(updateRequest.StoreName));
                Assert.That(product.GetProperty("product").GetString(), Is.EqualTo(updateRequest.ProductName));
                Assert.That(product.GetProperty("key").GetString(), Is.EqualTo(updateRequest.Key));
                Assert.That(product.GetProperty("owner").GetString(), Is.EqualTo(updateRequest.Owner));
                Assert.That(product.GetProperty("comment").GetString(), Is.EqualTo(updateRequest.Comment));
                Assert.That(product.GetProperty("status").GetString(), Is.EqualTo(updateRequest.Status));
            });
        }

        [Test]
        public async Task GivenAnExistingProductKey_WhenUpdatingWithEmptyFieldsAndUnknownStatus_ThenExistingValuesArePreserved()
        {
            await AddProductKeyAsync("OriginalStore", "OriginalProduct", "KEY-PRESERVE", "OriginalOwner", "OriginalComment", "Vacant");

            using HttpResponseMessage updateResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateUpdateRequest(new UpdateProductKeyRequest
                {
                    Key = "KEY-PRESERVE",
                    StoreName = " ",
                    ProductName = "",
                    Owner = null,
                    Comment = "",
                    Status = null
                }));
            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(updateResponse);

            using HttpResponseMessage getResponse = await Client.SendAsync(
                ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest
                {
                    Key = "KEY-PRESERVE",
                    Count = 1
                }));
            using JsonDocument result = await ProductKeyApiResponseAssertions.AssertSuccessfulResponseAsync(
                getResponse,
                1);
            JsonElement product = result.RootElement.GetProperty("products")[0];

            Assert.Multiple(() =>
            {
                Assert.That(product.GetProperty("store").GetString(), Is.EqualTo("OriginalStore"));
                Assert.That(product.GetProperty("product").GetString(), Is.EqualTo("OriginalProduct"));
                Assert.That(product.GetProperty("owner").GetString(), Is.EqualTo("OriginalOwner"));
                Assert.That(product.GetProperty("comment").GetString(), Is.EqualTo("OriginalComment"));
                Assert.That(product.GetProperty("status").GetString(), Is.EqualTo("Vacant"));
            });
        }

        [Test]
        public async Task GivenAnUnknownKey_WhenUpdating_ThenNotFoundIsReturned()
        {
            using HttpResponseMessage response = await Client.SendAsync(
                ProductKeyRequestFactory.CreateUpdateRequest(new UpdateProductKeyRequest
                {
                    Key = "KEY-DOES-NOT-EXIST",
                    Status = "Used"
                }));

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.NotFound);
        }

        private async Task AddProductKeyAsync(
            string storeName,
            string productName,
            string key,
            string owner,
            string comment,
            string status)
        {
            AddProductKeyRequest request = CreateAddRequest(
                storeName,
                productName,
                key,
                owner,
                comment,
                status);
            using HttpResponseMessage response = await Client.SendAsync(
                ProductKeyRequestFactory.CreateAddRequest(request));

            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(response);
        }

        private static AddProductKeyRequest CreateAddRequest(
            string storeName,
            string productName,
            string key,
            string owner,
            string comment,
            string? status)
            => new()
            {
                StoreName = storeName,
                ProductName = productName,
                Key = key,
                Owner = owner,
                Comment = comment,
                Status = status
            };

        private static GetProductKeyRequest CreateGetRequestWithFilter(
            string filterName,
            string filterValue)
        {
            GetProductKeyRequest request = new() { Count = 10 };

            if (filterName == "Store")
            {
                request.StoreName = filterValue;
            }
            else if (filterName == "Product")
            {
                request.ProductName = filterValue;
            }
            else if (filterName == "Key")
            {
                request.Key = filterValue;
            }
            else if (filterName == "Owner")
            {
                request.Owner = filterValue;
            }
            else
            {
                request.Status = filterValue;
            }

            return request;
        }

        private static string ExpectedStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status) || status == "NotARecognisedStatus")
            {
                return "Unknown";
            }

            return status;
        }

        private static string ExpectedKeyForFilter(string filterName)
        {
            if (filterName == "Key" || filterName == "Owner" || filterName == "Status")
            {
                return "KEY-002";
            }

            return "KEY-001";
        }
    }
}