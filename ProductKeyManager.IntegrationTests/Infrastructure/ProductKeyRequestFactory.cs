using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using NuciAPI.Middleware;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.IntegrationTests.Infrastructure
{
    internal static class ProductKeyRequestFactory
    {
        private static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNamingPolicy = null
        };

        public static HttpRequestMessage CreateAddRequest(AddProductKeyRequest requestBody)
            => CreateJsonRequest(HttpMethod.Post, requestBody);

        public static HttpRequestMessage CreateGetRequest(GetProductKeyRequest requestBody)
            => CreateJsonRequest(HttpMethod.Get, requestBody);

        public static HttpRequestMessage CreateUpdateRequest(UpdateProductKeyRequest requestBody)
            => CreateJsonRequest(HttpMethod.Put, requestBody);

        public static HttpRequestMessage CreateJsonRequest<TRequest>(HttpMethod method, TRequest requestBody)
        {
            HttpRequestMessage request = new(method, "/ProductKeys");
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody, JsonOptions),
                Encoding.UTF8,
                "application/json");
            request.Headers.TryAddWithoutValidation(
                "Authorization",
                $"Bearer {ProductKeyManagerApiFactory.ApiKey}");
            request.Headers.TryAddWithoutValidation(
                NuciApiHeaderNames.ClientId,
                "ProductKeyManager.IntegrationTests");
            request.Headers.TryAddWithoutValidation(
                NuciApiHeaderNames.RequestId,
                Guid.NewGuid().ToString("D").ToUpperInvariant());
            request.Headers.TryAddWithoutValidation(
                NuciApiHeaderNames.Timestamp,
                DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture));

            return request;
        }

        public static void SetHeader(HttpRequestMessage request, string headerName, string headerValue)
        {
            request.Headers.Remove(headerName);
            request.Headers.TryAddWithoutValidation(headerName, headerValue);
        }
    }
}