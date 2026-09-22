using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using NUnit.Framework;

using NuciAPI.Middleware;

using ProductKeyManager.Api.Models;
using ProductKeyManager.IntegrationTests.Assertions;
using ProductKeyManager.IntegrationTests.Infrastructure;

namespace ProductKeyManager.IntegrationTests
{
    [TestFixture]
    [NonParallelizable]
    public sealed class ProductKeysApiSecurityIntegrationTests : ProductKeyManagerApiTestBase
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase("Bearer")]
        [TestCase("Bearer ")]
        [TestCase("Basic Nucilandia-Integration-Test-Key")]
        [TestCase("Bearer Wrong-Key")]
        [TestCase("nucilandia-integration-test-key")]
        public async Task GivenAnInvalidAuthorisationHeader_WhenAddingAProductKey_ThenUnauthorisedIsReturned(
            string? authorisation)
        {
            using HttpRequestMessage request = ProductKeyRequestFactory.CreateAddRequest(CreateAddRequest());

            if (authorisation is null)
            {
                request.Headers.Remove("Authorization");
            }
            else
            {
                ProductKeyRequestFactory.SetHeader(request, "Authorization", authorisation);
            }

            using HttpResponseMessage response = await Client.SendAsync(request);

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.Unauthorized);
        }

        [TestCaseSource(nameof(GetProtocolHeaderNames))]
        public async Task GivenAMissingProtocolHeader_WhenAddingAProductKey_ThenBadRequestIsReturned(
            string headerName)
        {
            using HttpRequestMessage request = ProductKeyRequestFactory.CreateAddRequest(CreateAddRequest());
            request.Headers.Remove(headerName);

            using HttpResponseMessage response = await Client.SendAsync(request);

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.BadRequest);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1001)]
        [TestCase(int.MaxValue)]
        public async Task GivenAnOutOfRangeCount_WhenRetrievingProductKeys_ThenBadRequestIsReturned(
            int count)
        {
            using HttpRequestMessage request = ProductKeyRequestFactory.CreateGetRequest(new GetProductKeyRequest
            {
                Count = count
            });

            using HttpResponseMessage response = await Client.SendAsync(request);

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GivenMalformedJson_WhenRetrievingProductKeys_ThenBadRequestIsReturned()
        {
            using HttpRequestMessage request = ProductKeyRequestFactory.CreateJsonRequest(
                HttpMethod.Get,
                "not-json");

            using HttpResponseMessage response = await Client.SendAsync(request);

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GivenAnIdenticalRequestIdentifier_WhenRepeatingARequest_ThenConflictIsReturned()
        {
            using HttpRequestMessage firstRequest = ProductKeyRequestFactory.CreateAddRequest(CreateAddRequest());
            string requestIdentifier = firstRequest.Headers.GetValues(NuciApiHeaderNames.RequestId).Single();
            using HttpRequestMessage secondRequest = ProductKeyRequestFactory.CreateAddRequest(CreateAddRequest());
            ProductKeyRequestFactory.SetHeader(
                secondRequest,
                NuciApiHeaderNames.RequestId,
                requestIdentifier);

            using HttpResponseMessage firstResponse = await Client.SendAsync(firstRequest);
            using HttpResponseMessage secondResponse = await Client.SendAsync(secondRequest);

            await ProductKeyApiResponseAssertions.AssertMutationSucceededAsync(firstResponse);
            await ProductKeyApiResponseAssertions.AssertStatusAsync(secondResponse, HttpStatusCode.Conflict);
        }

        [Test]
        public async Task GivenAnUnknownRoute_WhenSendingAValidRequest_ThenNotFoundIsReturned()
        {
            using HttpRequestMessage request = ProductKeyRequestFactory.CreateJsonRequest(
                HttpMethod.Get,
                new GetProductKeyRequest { Count = 1 });
            request.RequestUri = new Uri("/NotAProductKeyRoute", UriKind.Relative);

            using HttpResponseMessage response = await Client.SendAsync(request);

            await ProductKeyApiResponseAssertions.AssertStatusAsync(response, HttpStatusCode.NotFound);
        }

        private static AddProductKeyRequest CreateAddRequest()
            => new()
            {
                StoreName = "Store",
                ProductName = "Product",
                Key = Guid.NewGuid().ToString("N"),
                Owner = "Owner",
                Comment = "Comment",
                Status = "Vacant"
            };

        private static IEnumerable<string> GetProtocolHeaderNames()
            =>
            [
                NuciApiHeaderNames.ClientId,
                NuciApiHeaderNames.RequestId,
                NuciApiHeaderNames.Timestamp
            ];
    }
}