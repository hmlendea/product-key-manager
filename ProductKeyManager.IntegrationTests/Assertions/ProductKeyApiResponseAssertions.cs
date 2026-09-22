using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using NUnit.Framework;

namespace ProductKeyManager.IntegrationTests.Assertions
{
    internal static class ProductKeyApiResponseAssertions
    {
        public static async Task<JsonDocument> AssertSuccessfulResponseAsync(
            HttpResponseMessage response,
            int expectedCount)
        {
            string content = await response.Content.ReadAsStringAsync();
            JsonDocument document = JsonDocument.Parse(content);
            JsonElement root = document.RootElement;

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), content);
                Assert.That(response.Content.Headers.ContentType?.MediaType, Is.EqualTo("application/json"));
                Assert.That(root.GetProperty("success").GetBoolean());
                Assert.That(root.GetProperty("products").GetArrayLength(), Is.EqualTo(expectedCount));
                Assert.That(root.GetProperty("count").GetInt32(), Is.EqualTo(expectedCount));
                Assert.That(root.GetProperty("hmac").GetString(), Is.Not.Null.And.Not.Empty);
            });

            return document;
        }

        public static async Task AssertMutationSucceededAsync(HttpResponseMessage response)
        {
            string content = await response.Content.ReadAsStringAsync();
            using JsonDocument document = JsonDocument.Parse(content);

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), content);
                Assert.That(document.RootElement.GetProperty("success").GetBoolean());
                Assert.That(document.RootElement.GetProperty("hmac").ValueKind, Is.EqualTo(JsonValueKind.Null));
            });
        }

        public static async Task AssertStatusAsync(
            HttpResponseMessage response,
            HttpStatusCode expectedStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(expectedStatusCode), content);
        }

        public static IEnumerable<JsonElement> GetProducts(JsonDocument document)
            => document.RootElement.GetProperty("products").EnumerateArray();
    }
}