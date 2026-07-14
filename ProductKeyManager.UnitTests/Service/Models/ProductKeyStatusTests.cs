using System.Linq;

using NUnit.Framework;

using ProductKeyManager.Service.Models;

namespace ProductKeyManager.UnitTests.Service.Models
{
    [TestFixture]
    public sealed class ProductKeyStatusTests
    {
        // ── GetValues ──────────────────────────────────────────────────────────────────

        [Test]
        public void GivenProductKeyStatus_WhenGetValuesIsCalled_ThenContainsAllSevenStatuses()
        {
            int count = ProductKeyStatus.GetValues().Length;

            Assert.That(count, Is.EqualTo(7));
        }

        // ── Name ────────────────────────────────────────────────────────────────────────

        [Test]
        public void GivenUnknownStatus_WhenNameIsAccessed_ThenReturnsUnknown()
        {
            Assert.That(ProductKeyStatus.Unknown.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void GivenUsedStatus_WhenNameIsAccessed_ThenReturnsUsed()
        {
            Assert.That(ProductKeyStatus.Used.Name, Is.EqualTo("Used"));
        }

        [Test]
        public void GivenVacantStatus_WhenNameIsAccessed_ThenReturnsVacant()
        {
            Assert.That(ProductKeyStatus.Vacant.Name, Is.EqualTo("Vacant"));
        }

        [Test]
        public void GivenInvalidStatus_WhenNameIsAccessed_ThenReturnsInvalid()
        {
            Assert.That(ProductKeyStatus.Invalid.Name, Is.EqualTo("Invalid"));
        }

        [Test]
        public void GivenAlreadyOwnedStatus_WhenNameIsAccessed_ThenReturnsAlreadyOwned()
        {
            Assert.That(ProductKeyStatus.AlreadyOwned.Name, Is.EqualTo("AlreadyOwned"));
        }

        [Test]
        public void GivenRequiresBaseProductStatus_WhenNameIsAccessed_ThenReturnsRequiresBaseProduct()
        {
            Assert.That(ProductKeyStatus.RequiresBaseProduct.Name, Is.EqualTo("RequiresBaseProduct"));
        }

        [Test]
        public void GivenRegionLockedStatus_WhenNameIsAccessed_ThenReturnsRegionLocked()
        {
            Assert.That(ProductKeyStatus.RegionLocked.Name, Is.EqualTo("RegionLocked"));
        }

        // ── FromName ───────────────────────────────────────────────────────────────────

        [Test]
        public void GivenNullName_WhenFromNameIsCalled_ThenReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(null);

            Assert.That(status.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void GivenEmptyStringName_WhenFromNameIsCalled_ThenReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(string.Empty);

            Assert.That(status.Name, Is.EqualTo("Unknown"));
        }

        [Test]
        public void GivenWhiteSpaceOnlyName_WhenFromNameIsCalled_ThenReturnsUnknown()
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
        public void GivenValidName_WhenFromNameIsCalled_ThenReturnsStatusWithMatchingName(string name)
        {
            ProductKeyStatus status = ProductKeyStatus.FromName(name);

            Assert.That(status.Name, Is.EqualTo(name));
        }

        [Test]
        public void GivenUnrecognisedName_WhenFromNameIsCalled_ThenReturnsUnknown()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("Nucilandia");

            Assert.That(status, Is.EqualTo(ProductKeyStatus.Unknown));
        }

        // ── Equals ───────────────────────────────────────────────────────────────────────

        [Test]
        public void GivenSameReference_WhenEqualsIsCalled_ThenReturnsTrue()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("Used");

            Assert.That(status.Equals(status), Is.True);
        }

        [Test]
        public void GivenDifferentInstanceWithSameName_WhenEqualsIsCalled_ThenReturnsTrue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Vacant");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Vacant");

            Assert.That(firstStatus.Equals(secondStatus), Is.True);
        }

        [Test]
        public void GivenDifferentName_WhenEqualsIsCalled_ThenReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals(ProductKeyStatus.Vacant), Is.False);
        }

        [Test]
        public void GivenNull_WhenEqualsIsCalled_ThenReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals((ProductKeyStatus)null), Is.False);
        }

        [Test]
        public void GivenObjectOfSameTypeWithSameName_WhenEqualsIsCalled_ThenReturnsTrue()
        {
            ProductKeyStatus status = ProductKeyStatus.FromName("Used");
            object boxedStatus = ProductKeyStatus.FromName("Used");

            Assert.That(status.Equals(boxedStatus), Is.True);
        }

        [Test]
        public void GivenObjectOfDifferentType_WhenEqualsIsCalled_ThenReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals("Used"), Is.False);
        }

        [Test]
        public void GivenNullObject_WhenEqualsIsCalled_ThenReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used.Equals((object)null), Is.False);
        }

        // ── operator == / != ────────────────────────────────────────────────────────

        [Test]
        public void GivenTwoStatusesOfSameName_WhenOperatorEqualsIsUsed_ThenReturnsTrue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Invalid");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Invalid");

            Assert.That(firstStatus == secondStatus, Is.True);
        }

        [Test]
        public void GivenTwoStatusesOfDifferentNames_WhenOperatorEqualsIsUsed_ThenReturnsFalse()
        {
            Assert.That(ProductKeyStatus.Used == ProductKeyStatus.Vacant, Is.False);
        }

        [Test]
        public void GivenBothNull_WhenOperatorEqualsIsUsed_ThenReturnsTrue()
        {
            ProductKeyStatus firstStatus = null;
            ProductKeyStatus secondStatus = null;

            Assert.That(firstStatus == secondStatus, Is.True);
        }

        [Test]
        public void GivenFirstNullAndSecondNotNull_WhenOperatorEqualsIsUsed_ThenReturnsFalse()
        {
            ProductKeyStatus firstStatus = null;

            Assert.That(firstStatus == ProductKeyStatus.Used, Is.False);
        }

        [Test]
        public void GivenFirstNotNullAndSecondNull_WhenOperatorEqualsIsUsed_ThenReturnsFalse()
        {
            ProductKeyStatus secondStatus = null;

            Assert.That(ProductKeyStatus.Used == secondStatus, Is.False);
        }

        [Test]
        public void GivenTwoStatusesOfSameName_WhenOperatorNotEqualsIsUsed_ThenReturnsFalse()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("RegionLocked");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("RegionLocked");

            Assert.That(firstStatus != secondStatus, Is.False);
        }

        [Test]
        public void GivenTwoStatusesOfDifferentNames_WhenOperatorNotEqualsIsUsed_ThenReturnsTrue()
        {
            Assert.That(ProductKeyStatus.Used != ProductKeyStatus.Invalid, Is.True);
        }

        // ── ToString ───────────────────────────────────────────────────────────────────

        [Test]
        public void GivenUsedStatus_WhenToStringIsCalled_ThenReturnsName()
        {
            Assert.That(ProductKeyStatus.Used.ToString(), Is.EqualTo("Used"));
        }

        // ── GetHashCode ─────────────────────────────────────────────────────────────

        [Test]
        public void GivenTwoInstancesOfSameName_WhenGetHashCodeIsCalled_ThenReturnsSameValue()
        {
            ProductKeyStatus firstStatus = ProductKeyStatus.FromName("Vacant");
            ProductKeyStatus secondStatus = ProductKeyStatus.FromName("Vacant");

            Assert.That(firstStatus.GetHashCode(), Is.EqualTo(secondStatus.GetHashCode()));
        }

        [Test]
        public void GivenUsedStatus_WhenGetHashCodeIsCalled_ThenDoesNotThrow()
        {
            Assert.That(() => ProductKeyStatus.Used.GetHashCode(), Throws.Nothing);
        }
    }
}
