using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using ProductKeyManager.Service.Models;

namespace ProductKeyManager.UnitTests.Service.Models
{
    [TestFixture]
    public sealed class ProductKeyStatusTests
    {
        [Test]
        public void Values_ContainsAllSevenStatuses()
        {
            int count = ProductKeyStatus.Values.Count();

            Assert.That(count, Is.EqualTo(7));
        }

        [Test]
        public void Unknown_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.Unknown.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void Used_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.Used.Name, Is.EqualTo("Used"));
        }

        [Test]
        public void Vacant_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.Vacant.Name, Is.EqualTo("Vacant"));
        }

        [Test]
        public void Invalid_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.Invalid.Name, Is.EqualTo("Invalid"));
        }

        [Test]
        public void AlreadyOwned_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.AlreadyOwned.Name, Is.EqualTo("AlreadyOwned"));
        }

        [Test]
        public void RequiresBaseProduct_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.RequiresBaseProduct.Name, Is.EqualTo("RequiresBaseProduct"));
        }

        [Test]
        public void RegionLocked_HasCorrectName()
        {
            Assert.That(ProductKeyStatus.RegionLocked.Name, Is.EqualTo("RegionLocked"));
        }

        [Test]
        public void FromName_WithNull_ReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(null);

            Assert.That(status.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void FromName_WithEmptyString_ReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(string.Empty);

            Assert.That(status.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void FromName_WithWhiteSpaceOnly_ReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("   ");

            Assert.That(status.Name, Is.EqualTo("Unknown"));
        }

        [TestCase("Unknown")]
        [TestCase("Used")]
        [TestCase("Vacant")]
        [TestCase("Invalid")]
        [TestCase("AlreadyOwned")]
        [TestCase("RequiresBaseProduct")]
        [TestCase("RegionLocked")]
        public void FromName_WithValidName_ReturnsStatusWithMatchingName(string name)
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(name);

            Assert.That(status.Name, Is.EqualTo(name));
        }

        [Test]
        public void FromName_WithUnrecognisedName_ThrowsKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => ProductKeyStatus.FromName("Nucilandia"));
        }

        [Test]
        public void Equals_WithSameReference_ReturnsTrue()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("Used");

            Assert.That(status.Equals(status), Is.True);
        }

        [Test]
        public void Equals_WithDifferentInstanceAndSameName_ReturnsTrue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Vacant");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Vacant");

            Assert.That(firstStatus.Equals(secondStatus), Is.True);
        }

        [Test]
        public void Equals_WithDifferentName_ReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals(ProductKeyStatus.Vacant), Is.False);
        }

        [Test]
        public void Equals_WithNull_ReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals((ProductKeyStatus)null), Is.False);
        }

        [Test]
        public void Equals_WithObjectOfSameTypeAndSameName_ReturnsTrue()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("Used");
            object boxedStatus = ProductKeyStatus.FromName("Used");

            Assert.That(status.Equals(boxedStatus), Is.True);
        }

        [Test]
        public void Equals_WithObjectOfDifferentType_ReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals("Used"), Is.False);
        }

        [Test]
        public void Equals_WithNullObject_ReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals((object)null), Is.False);
        }

        [Test]
        public void OperatorEquals_WithTwoStatusesOfSameName_ReturnsTrue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Invalid");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Invalid");

            Assert.That(firstStatus == secondStatus, Is.True);
        }

        [Test]
        public void OperatorEquals_WithTwoStatusesOfDifferentNames_ReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used == ProductKeyStatus.Vacant, Is.False);
        }

        [Test]
        public void OperatorEquals_WithBothNull_ReturnsTrue()
        {
            ProductKeyStatus firstStatus = null;
            ProductKeyStatus secondStatus = null;

            Assert.That(firstStatus == secondStatus, Is.True);
        }

        [Test]
        public void OperatorEquals_WithFirstNullAndSecondNotNull_ReturnsFalse()
        {
            ProductKeyStatus firstStatus = null;

            Assert.That(firstStatus == ProductKeyStatus.Used, Is.False);
        }

        [Test]
        public void OperatorEquals_WithFirstNotNullAndSecondNull_ReturnsFalse()
        {
            ProductKeyStatus secondStatus = null;

            Assert.That(ProductKeyStatus.Used == secondStatus, Is.False);
        }

        [Test]
        public void OperatorNotEquals_WithTwoStatusesOfSameName_ReturnsFalse()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("RegionLocked");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("RegionLocked");

            Assert.That(firstStatus != secondStatus, Is.False);
        }

        [Test]
        public void OperatorNotEquals_WithTwoStatusesOfDifferentNames_ReturnsTrue()
        {
            Assert.That(ProductKeyStatus.Used != ProductKeyStatus.Invalid, Is.True);
        }

        [Test]
        public void ToString_ReturnsName()
        {
            Assert.That(ProductKeyStatus.Used.ToString(), Is.EqualTo("Used"));
        }

        [Test]
        public void GetHashCode_WithTwoInstancesOfSameName_ReturnsSameValue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Vacant");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Vacant");

            Assert.That(firstStatus.GetHashCode(), Is.EqualTo(secondStatus.GetHashCode()));
        }

        [Test]
        public void GetHashCode_DoesNotThrow()
        {
            Assert.That(() => ProductKeyStatus.Used.GetHashCode(), Throws.Nothing);
        }
    }
}
