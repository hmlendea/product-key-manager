using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.UnitTests.Api.Models
{
    [TestFixture]
    public sealed class GetProductKeyResponseTests
    {
        [Test]
        public void Constructor_WithSingleProductKeyObject_SetsProductKeysToCollectionWithThatObject()
        {
            ProductKeyObject productKeyObject = new() { Key = "DARK-SOUL-S613-MNOP", Store = "NucilandiaSteam" };

            GetProductKeyResponse response = new(productKeyObject);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Key, Is.EqualTo("DARK-SOUL-S613-MNOP"));
        }

        [Test]
        public void Constructor_WithCollectionOfProductKeyObjects_SetsProductKeysToThatCollection()
        {
            List<ProductKeyObject> productKeyObjects = new()
            {
                new() { Key = "DARK-SOUL-S613-AAA1" },
                new() { Key = "DARK-SOUL-S613-BBB2" },
                new() { Key = "DARK-SOUL-S613-CCC3" }
            };

            GetProductKeyResponse response = new(productKeyObjects);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Constructor_WithEmptyCollection_SetsProductKeysToEmptyCollection()
        {
            List<ProductKeyObject> productKeyObjects = new();

            GetProductKeyResponse response = new(productKeyObjects);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Count_WithSingleProductKeyObject_ReturnsOne()
        {
            ProductKeyObject productKeyObject = new() { Key = "DARK-SOUL-S613-MNOP" };

            GetProductKeyResponse response = new(productKeyObject);

            Assert.That(response.Count, Is.EqualTo(1));
        }

        [Test]
        public void Count_WithThreeProductKeyObjects_ReturnsThree()
        {
            List<ProductKeyObject> productKeyObjects = new()
            {
                new() { Key = "DARK-SOUL-S613-AAA1" },
                new() { Key = "DARK-SOUL-S613-BBB2" },
                new() { Key = "DARK-SOUL-S613-CCC3" }
            };

            GetProductKeyResponse response = new(productKeyObjects);

            Assert.That(response.Count, Is.EqualTo(3));
        }

        [Test]
        public void Count_WithEmptyCollection_ReturnsZero()
        {
            GetProductKeyResponse response = new(new List<ProductKeyObject>());

            Assert.That(response.Count, Is.EqualTo(0));
        }

        [Test]
        public void Constructor_WithSingleObject_PreservesAllProperties()
        {
            ProductKeyObject productKeyObject = new()
            {
                Store = "NucilandiaSteam",
                Product = "DarkSouls",
                Key = "DARK-SOUL-S613-MNOP",
                Owner = "solaire_of_astora",
                Comment = "A fine key from Astora.",
                Status = "Vacant"
            };

            GetProductKeyResponse response = new(productKeyObject);

            ProductKeyObject returnedObject = response.ProductKeys.First();
            Assert.That(returnedObject.Store, Is.EqualTo("NucilandiaSteam"));
            Assert.That(returnedObject.Product, Is.EqualTo("DarkSouls"));
            Assert.That(returnedObject.Key, Is.EqualTo("DARK-SOUL-S613-MNOP"));
            Assert.That(returnedObject.Owner, Is.EqualTo("solaire_of_astora"));
            Assert.That(returnedObject.Comment, Is.EqualTo("A fine key from Astora."));
            Assert.That(returnedObject.Status, Is.EqualTo("Vacant"));
        }
    }
}
