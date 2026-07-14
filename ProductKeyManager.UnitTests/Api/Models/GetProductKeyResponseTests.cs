using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ProductKeyManager.Api.Models;

namespace ProductKeyManager.UnitTests.Api.Models
{
    [TestFixture]
    public sealed class GetProductKeyResponseTests
    {
        // ── Constructor ───────────────────────────────────────────────────────────

        [Test]
        public void GivenSingleProductKeyObject_WhenConstructorIsCalled_ThenSetsProductKeysToCollectionWithThatObject()
        {
            ProductKeyObject productKeyObject = new() { Key = "DARK-SOUL-S613-MNOP", Store = "NucilandiaSteam" };

            GetProductKeyResponse response = new(productKeyObject);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Key, Is.EqualTo("DARK-SOUL-S613-MNOP"));
        }

        [Test]
        public void GivenCollectionOfProductKeyObjects_WhenConstructorIsCalled_ThenSetsProductKeysToThatCollection()
        {
            IEnumerable<ProductKeyObject> productKeyObjects =
            [
                new() { Key = "DARK-SOUL-S613-AAA1" },
                new() { Key = "DARK-SOUL-S613-BBB2" },
                new() { Key = "DARK-SOUL-S613-CCC3" }
            ];

            GetProductKeyResponse response = new(productKeyObjects);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GivenEmptyCollection_WhenConstructorIsCalled_ThenSetsProductKeysToEmptyCollection()
        {
            GetProductKeyResponse response = new([]);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(0));
        }

        // ── Count ────────────────────────────────────────────────────────────────────

        [Test]
        public void GivenSingleProductKeyObject_WhenCountIsRead_ThenReturnsOne()
        {
            ProductKeyObject productKeyObject = new() { Key = "DARK-SOUL-S613-MNOP" };

            GetProductKeyResponse response = new(productKeyObject);

            Assert.That(response.Count, Is.EqualTo(1));
        }

        [Test]
        public void GivenThreeProductKeyObjects_WhenCountIsRead_ThenReturnsThree()
        {
            IEnumerable<ProductKeyObject> productKeyObjects =
            [
                new() { Key = "DARK-SOUL-S613-AAA1" },
                new() { Key = "DARK-SOUL-S613-BBB2" },
                new() { Key = "DARK-SOUL-S613-CCC3" }
            ];

            GetProductKeyResponse response = new(productKeyObjects);

            Assert.That(response.Count, Is.EqualTo(3));
        }

        [Test]
        public void GivenEmptyCollection_WhenCountIsRead_ThenReturnsZero()
        {
            GetProductKeyResponse response = new([]);

            Assert.That(response.Count, Is.EqualTo(0));
        }

        [Test]
        public void GivenSingleObjectWithAllProperties_WhenConstructorIsCalled_ThenPreservesAllProperties()
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
