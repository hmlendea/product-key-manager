using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using NSubstitute;

using NuciDAL.Repositories;

using NuciLog.Core;

using NUnit.Framework;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service;

namespace ProductKeyManager.UnitTests.Service
{
    [TestFixture]
    public sealed class ProductKeyServiceTests
    {
        private static string DateTimeFormat => "yyyy.MM.ddTHH:mm:ss.ffffzzz";
        private static string TestAddedDateTime => new DateTime(2012, 9, 5, 0, 0, 0, DateTimeKind.Utc).ToString(DateTimeFormat);

        IFileRepository<ProductKeyDataObject> repository;
        SecuritySettings securitySettings;
        ILogger logger;
        ProductKeyService service;

        [SetUp]
        public void SetUp()
        {
            repository = Substitute.For<IFileRepository<ProductKeyDataObject>>();
            securitySettings = new() { SharedSecretKey = "nucilandia-test-secret-key" };
            logger = Substitute.For<ILogger>();
            service = new ProductKeyService(repository, securitySettings, logger);
        }

        // ── GetProductKey ─────────────────────────────────────────────────────────────

        [Test]
        public void GivenRepositoryHasOneMatchingEntity_WhenGetProductKeyIsCalled_ThenReturnsResponseWithOneKey()
        {
            ProductKeyDataObject entity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            IEnumerable<ProductKeyDataObject> entities = [entity];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 1 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GivenRepositoryHasMultipleMatchingEntities_WhenGetProductKeyIsCalled_ThenReturnsAllMatchingKeys()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 3 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GivenRepositoryIsEmpty_WhenGetProductKeyIsCalled_ThenThrowsNullReferenceException()
        {
            IEnumerable<ProductKeyDataObject> emptyEntities = [];
            repository.GetAll().Returns(emptyEntities);

            GetProductKeyRequest request = new() { Count = 1 };

            Assert.Throws<NullReferenceException>(() => service.GetProductKey(request));
        }

        [Test]
        public void GivenMultipleEntitiesMatch_WhenGetProductKeyIsCalledWithCountOne_ThenReturnsOneKey()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 1 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GivenFiveMatchingEntities_WhenGetProductKeyIsCalledWithCountTwo_ThenReturnsTwoKeys()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-DDD4", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-EEE5", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 2 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GivenTwoMatchingEntities_WhenGetProductKeyIsCalledWithCountOneHundred_ThenReturnsBothMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 100 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GivenEntitiesWithMixedAttributes_WhenGetProductKeyIsCalledWithNullFilters_ThenReturnsAllEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaGog", "BloodBorne", "Used"),
                CreateTestEntity("DARK-SOUL-S873-CCC3", "NucilandiaSteam", "Sekiro", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GivenEntitiesWithDifferentStoreNames_WhenGetProductKeyIsCalledWithStoreNameFilter_ThenReturnsOnlyMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "NucilandiaSteam", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.All(key => key.Store == "NucilandiaSteam"), Is.True);
        }

        [Test]
        public void GivenEntitiesWithDifferentProductNames_WhenGetProductKeyIsCalledWithProductNameFilter_ThenReturnsOnlyMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DS-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("BB-KEY-S613-BBB2", "NucilandiaSteam", "BloodBorne", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { ProductName = "DarkSouls", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Product, Is.EqualTo("DarkSouls"));
        }

        [Test]
        public void GivenEntitiesWithDifferentKeys_WhenGetProductKeyIsCalledWithKeyFilter_ThenReturnsOnlyMatchingEntity()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Key = "DARK-SOUL-S613-AAA1", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Key, Is.EqualTo("DARK-SOUL-S613-AAA1"));
        }

        [Test]
        public void GivenEntitiesWithDifferentOwners_WhenGetProductKeyIsCalledWithOwnerFilter_ThenReturnsOnlyMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Used", owner: "solaire_of_astora"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Used", owner: "IlarionPintilie")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Owner = "solaire_of_astora", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Owner, Is.EqualTo("solaire_of_astora"));
        }

        [Test]
        public void GivenEntitiesWithDifferentStatuses_WhenGetProductKeyIsCalledWithStatusFilter_ThenReturnsOnlyMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Used")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Status = "Vacant", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Status, Is.EqualTo("Vacant"));
        }

        [Test]
        public void GivenEntitiesWithDifferentStoreNames_WhenGetProductKeyIsCalledWithStartsWithRegexFilter_ThenReturnsMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant"),
                CreateTestEntity("EPIC-KEY-S613-CCC3", "EpicGames", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "^Nucilandia", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GivenEntitiesWithDifferentStoreNames_WhenGetProductKeyIsCalledWithEndsWithRegexFilter_ThenReturnsMatchingEntities()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "Steam$", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Store, Is.EqualTo("NucilandiaSteam"));
        }

        [Test]
        public void GivenEntityWithNullStoreName_WhenGetProductKeyIsCalledWithStoreNameFilter_ThenExcludesThatEntity()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", null, "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "NucilandiaSteam", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GivenEntityWithNullOwner_WhenGetProductKeyIsCalledWithOwnerFilter_ThenExcludesThatEntity()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Used", owner: "solaire_of_astora"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant", owner: null)
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Owner = "solaire_of_astora", Count = 10 };

            GetProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GivenNoEntitiesMatchFilters_WhenGetProductKeyIsCalled_ThenThrowsNullReferenceException()
        {
            IEnumerable<ProductKeyDataObject> entities =
            [
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant")
            ];
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "Astora", Count = 1 };

            Assert.Throws<NullReferenceException>(() => service.GetProductKey(request));
        }

        // ── AddProductKey ─────────────────────────────────────────────────────────────

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenCallsRepositoryAdd()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Any<ProductKeyDataObject>());
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenCallsRepositorySaveChanges()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).SaveChanges();
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectId()
        {
            string key = "DARK-SOUL-S613-MNOP";
            string expectedId = ComputeKeyId(key);
            AddProductKeyRequest request = CreateAddRequest(key, "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.Id == expectedId));
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectKey()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.Key == "DARK-SOUL-S613-MNOP"));
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectStoreName()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectProductName()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.ProductName == "DarkSouls"));
        }

        [Test]
        public void GivenRequestHasOwner_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectOwner()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                owner: "solaire_of_astora");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.Owner == "solaire_of_astora"));
        }

        [Test]
        public void GivenRequestHasComment_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectComment()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                comment: "A gift from Astora.");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.Comment == "A gift from Astora."));
        }

        [Test]
        public void GivenRequestHasStatus_WhenAddProductKeyIsCalled_ThenAddsEntityWithCorrectStatus()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                status: "Vacant");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenAddsEntityWithNonEmptyAddedDateTime()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyDataObject>(entity => !string.IsNullOrEmpty(entity.AddedDateTime)));
        }

        [Test]
        public void GivenValidRequest_WhenAddProductKeyIsCalled_ThenUpdatedDateTimeEqualsAddedDateTime()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");
            ProductKeyDataObject capturedEntity = null;
            repository.When(r => r.Add(Arg.Any<ProductKeyDataObject>()))
                .Do(call => capturedEntity = call.Arg<ProductKeyDataObject>());

            service.AddProductKey(request);

            Assert.That(capturedEntity.UpdatedDateTime, Is.EqualTo(capturedEntity.AddedDateTime));
        }

        // ── UpdateProductKey ──────────────────────────────────────────────────────────

        [Test]
        public void GivenNewStoreName_WhenUpdateProductKeyIsCalled_ThenUpdatesStoreName()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "OldStore", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                StoreName = "NucilandiaSteam"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void GivenNewProductName_WhenUpdateProductKeyIsCalled_ThenUpdatesProductName()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "OldProduct", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                ProductName = "DarkSoulsIII"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.ProductName == "DarkSoulsIII"));
        }

        [Test]
        public void GivenNewOwner_WhenUpdateProductKeyIsCalled_ThenUpdatesOwner()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Owner = "solaire_of_astora"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Owner == "solaire_of_astora"));
        }

        [Test]
        public void GivenNewComment_WhenUpdateProductKeyIsCalled_ThenUpdatesComment()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Comment = "Updated by Solaire."
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Comment == "Updated by Solaire."));
        }

        [Test]
        public void GivenNewNonUnknownStatus_WhenUpdateProductKeyIsCalled_ThenUpdatesStatus()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Status == "Used"));
        }

        [Test]
        public void GivenEmptyStoreName_WhenUpdateProductKeyIsCalled_ThenPreservesExistingStoreName()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                StoreName = string.Empty
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void GivenNullProductName_WhenUpdateProductKeyIsCalled_ThenPreservesExistingProductName()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                ProductName = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.ProductName == "DarkSouls"));
        }

        [Test]
        public void GivenNullOwner_WhenUpdateProductKeyIsCalled_ThenPreservesExistingOwner()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant",
                owner: "IlarionPintilie");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Owner = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Owner == "IlarionPintilie"));
        }

        [Test]
        public void GivenWhiteSpaceComment_WhenUpdateProductKeyIsCalled_ThenPreservesExistingComment()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant",
                comment: "Original comment.");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Comment = "   "
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Comment == "Original comment."));
        }

        [Test]
        public void GivenUnknownStatus_WhenUpdateProductKeyIsCalled_ThenPreservesExistingStatus()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Unknown"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void GivenNullStatus_WhenUpdateProductKeyIsCalled_ThenPreservesExistingStatus()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void GivenValidRequest_WhenUpdateProductKeyIsCalled_ThenCallsRepositoryUpdate()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Any<ProductKeyDataObject>());
        }

        [Test]
        public void GivenValidRequest_WhenUpdateProductKeyIsCalled_ThenCallsRepositorySaveChanges()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };

            service.UpdateProductKey(request);

            repository.Received(1).SaveChanges();
        }

        [Test]
        public void GivenValidRequest_WhenUpdateProductKeyIsCalled_ThenSetsUpdatedDateTimeToRecentValue()
        {
            ProductKeyDataObject existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };
            DateTime beforeUpdate = DateTime.Now.AddSeconds(-1);

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyDataObject>(entity =>
                DateTime.ParseExact(entity.UpdatedDateTime, DateTimeFormat, CultureInfo.InvariantCulture) >= beforeUpdate));
        }

        [Test]
        public void GivenValidRequest_WhenUpdateProductKeyIsCalled_ThenUsesKeyToLookUpEntityById()
        {
            string key = "DARK-SOUL-S613-MNOP";
            string expectedId = ComputeKeyId(key);
            ProductKeyDataObject existingEntity = CreateTestEntity(key, "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new() { Key = key, Status = "Used" };

            service.UpdateProductKey(request);

            repository.Received(1).Get(expectedId);
        }

        private static ProductKeyDataObject CreateTestEntity(
            string key,
            string storeName,
            string productName,
            string status,
            string owner = null,
            string comment = null) => new()
        {
            Id = ComputeKeyId(key),
            StoreName = storeName,
            ProductName = productName,
            Key = key,
            Owner = owner,
            ConfirmationCode = null,
            Comment = comment,
            Status = status,
            AddedDateTime = TestAddedDateTime,
            UpdatedDateTime = TestAddedDateTime
        };

        private static AddProductKeyRequest CreateAddRequest(
            string key,
            string storeName,
            string productName,
            string owner = null,
            string comment = null,
            string status = "Vacant") => new()
        {
            Key = key,
            StoreName = storeName,
            ProductName = productName,
            Owner = owner,
            Comment = comment,
            Status = status
        };

        private static string ComputeKeyId(string key)
            => new Guid(MD5.HashData(Encoding.Default.GetBytes(key))).ToString();
    }
}
