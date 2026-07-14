using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using NSubstitute;
using NUnit.Framework;

using NuciDAL.Repositories;
using NuciLog.Core;

using ProductKeyManager.Api.Models;
using ProductKeyManager.Configuration;
using ProductKeyManager.DataAccess.DataObjects;
using ProductKeyManager.Service;
using ProductKeyManager.Service.Models;

namespace ProductKeyManager.UnitTests.Service
{
    [TestFixture]
    public sealed class ProductKeyServiceTests
    {
        static string DateTimeFormat => "yyyy.MM.ddTHH:mm:ss.ffffzzz";
        static string TestAddedDateTime => new DateTime(2012, 9, 5, 0, 0, 0, DateTimeKind.Utc).ToString(DateTimeFormat);

        IFileRepository<ProductKeyEntity> repository;
        SecuritySettings securitySettings;
        ILogger logger;
        ProductKeyService service;

        [SetUp]
        public void SetUp()
        {
            repository = Substitute.For<IFileRepository<ProductKeyEntity>>();
            securitySettings = new() { SharedSecretKey = "nucilandia-test-secret-key" };
            logger = Substitute.For<ILogger>();
            service = new ProductKeyService(repository, securitySettings, logger);
        }

        [Test]
        public void GetProductKey_WhenRepositoryHasOneMatchingEntity_ReturnsResponseWithOneKey()
        {
            ProductKeyEntity entity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.GetAll().Returns(new List<ProductKeyEntity> { entity });

            GetProductKeyRequest request = new() { Count = 1 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetProductKey_WhenRepositoryHasMultipleMatchingEntities_ReturnsAllMatchingKeys()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 3 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetProductKey_WhenRepositoryIsEmpty_ThrowsNullReferenceException()
        {
            repository.GetAll().Returns(new List<ProductKeyEntity>());

            GetProductKeyRequest request = new() { Count = 1 };

            Assert.Throws<NullReferenceException>(() => service.GetProductKey(request));
        }

        [Test]
        public void GetProductKey_WithCountOne_WhenMultipleEntitiesMatch_ReturnsOneKey()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 1 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetProductKey_WithCountLessThanMatchingEntities_ReturnsOnlyCount()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-CCC3", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-DDD4", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-EEE5", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 2 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetProductKey_WithCountGreaterThanMatchingEntities_ReturnsAllMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S613-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 100 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetProductKey_WithNullFilters_ReturnsAllEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaGog", "BloodBorne", "Used"),
                CreateTestEntity("DARK-SOUL-S873-CCC3", "NucilandiaSteam", "Sekiro", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(3));
        }

        [Test]
        public void GetProductKey_WithStoreNameFilter_ReturnsOnlyMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "NucilandiaSteam", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.All(key => key.Store == "NucilandiaSteam"), Is.True);
        }

        [Test]
        public void GetProductKey_WithProductNameFilter_ReturnsOnlyMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DS-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("BB-KEY-S613-BBB2", "NucilandiaSteam", "BloodBorne", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { ProductName = "DarkSouls", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Product, Is.EqualTo("DarkSouls"));
        }

        [Test]
        public void GetProductKey_WithKeyFilter_ReturnsOnlyMatchingEntity()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Key = "DARK-SOUL-S613-AAA1", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Key, Is.EqualTo("DARK-SOUL-S613-AAA1"));
        }

        [Test]
        public void GetProductKey_WithOwnerFilter_ReturnsOnlyMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Used", owner: "solaire_of_astora"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Used", owner: "IlarionPintilie")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Owner = "solaire_of_astora", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Owner, Is.EqualTo("solaire_of_astora"));
        }

        [Test]
        public void GetProductKey_WithStatusFilter_ReturnsOnlyMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Used")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Status = "Vacant", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Status, Is.EqualTo("Vacant"));
        }

        [Test]
        public void GetProductKey_WithStoreNameStartsWithRegexFilter_ReturnsMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant"),
                CreateTestEntity("EPIC-KEY-S613-CCC3", "EpicGames", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "^Nucilandia", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetProductKey_WithStoreNameEndsWithRegexFilter_ReturnsMatchingEntities()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", "NucilandiaGog", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "Steam$", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
            Assert.That(response.ProductKeys.First().Store, Is.EqualTo("NucilandiaSteam"));
        }

        [Test]
        public void GetProductKey_WithStoreNameFilter_WhenEntityHasNullStoreName_ExcludesThatEntity()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("STEAM-KEY-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant"),
                CreateTestEntity("GOG-KEY-S613-BBB2", null, "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "NucilandiaSteam", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetProductKey_WithOwnerFilter_WhenEntityHasNullOwner_ExcludesThatEntity()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Used", owner: "solaire_of_astora"),
                CreateTestEntity("DARK-SOUL-S873-BBB2", "NucilandiaSteam", "DarkSouls", "Vacant", owner: null)
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { Owner = "solaire_of_astora", Count = 10 };

            ProductKeyResponse response = service.GetProductKey(request);

            Assert.That(response.ProductKeys.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetProductKey_WhenNoEntitiesMatchFilters_ThrowsNullReferenceException()
        {
            List<ProductKeyEntity> entities = new()
            {
                CreateTestEntity("DARK-SOUL-S613-AAA1", "NucilandiaSteam", "DarkSouls", "Vacant")
            };
            repository.GetAll().Returns(entities);

            GetProductKeyRequest request = new() { StoreName = "Astora", Count = 1 };

            Assert.Throws<NullReferenceException>(() => service.GetProductKey(request));
        }

        [Test]
        public void AddProductKey_WithValidRequest_CallsRepositoryAdd()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Any<ProductKeyEntity>());
        }

        [Test]
        public void AddProductKey_WithValidRequest_CallsRepositorySaveChanges()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).SaveChanges();
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithCorrectId()
        {
            string key = "DARK-SOUL-S613-MNOP";
            string expectedId = ComputeKeyId(key);
            AddProductKeyRequest request = CreateAddRequest(key, "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.Id == expectedId));
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithCorrectKey()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.Key == "DARK-SOUL-S613-MNOP"));
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithCorrectStoreName()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithCorrectProductName()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.ProductName == "DarkSouls"));
        }

        [Test]
        public void AddProductKey_WithOwnerInRequest_AddsEntityWithCorrectOwner()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                owner: "solaire_of_astora");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.Owner == "solaire_of_astora"));
        }

        [Test]
        public void AddProductKey_WithCommentInRequest_AddsEntityWithCorrectComment()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                comment: "A gift from Astora.");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.Comment == "A gift from Astora."));
        }

        [Test]
        public void AddProductKey_WithStatusInRequest_AddsEntityWithCorrectStatus()
        {
            AddProductKeyRequest request = CreateAddRequest(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls",
                status: "Vacant");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithNonEmptyAddedDateTime()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");

            service.AddProductKey(request);

            repository.Received(1).Add(Arg.Is<ProductKeyEntity>(entity => !string.IsNullOrEmpty(entity.AddedDateTime)));
        }

        [Test]
        public void AddProductKey_WithValidRequest_AddsEntityWithUpdatedDateTimeEqualToAddedDateTime()
        {
            AddProductKeyRequest request = CreateAddRequest("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls");
            ProductKeyEntity capturedEntity = null;
            repository.When(r => r.Add(Arg.Any<ProductKeyEntity>()))
                .Do(call => capturedEntity = call.Arg<ProductKeyEntity>());

            service.AddProductKey(request);

            Assert.That(capturedEntity.UpdatedDateTime, Is.EqualTo(capturedEntity.AddedDateTime));
        }

        [Test]
        public void UpdateProductKey_WithNewStoreName_UpdatesStoreName()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "OldStore", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                StoreName = "NucilandiaSteam"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void UpdateProductKey_WithNewProductName_UpdatesProductName()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "OldProduct", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                ProductName = "DarkSoulsIII"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.ProductName == "DarkSoulsIII"));
        }

        [Test]
        public void UpdateProductKey_WithNewOwner_UpdatesOwner()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Owner = "solaire_of_astora"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Owner == "solaire_of_astora"));
        }

        [Test]
        public void UpdateProductKey_WithNewComment_UpdatesComment()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Comment = "Updated by Solaire."
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Comment == "Updated by Solaire."));
        }

        [Test]
        public void UpdateProductKey_WithNewNonUnknownStatus_UpdatesStatus()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Status == "Used"));
        }

        [Test]
        public void UpdateProductKey_WithEmptyStoreName_PreservesExistingStoreName()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                StoreName = string.Empty
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.StoreName == "NucilandiaSteam"));
        }

        [Test]
        public void UpdateProductKey_WithNullProductName_PreservesExistingProductName()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                ProductName = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.ProductName == "DarkSouls"));
        }

        [Test]
        public void UpdateProductKey_WithNullOwner_PreservesExistingOwner()
        {
            ProductKeyEntity existingEntity = CreateTestEntity(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant",
                owner: "IlarionPintilie");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Owner = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Owner == "IlarionPintilie"));
        }

        [Test]
        public void UpdateProductKey_WithWhiteSpaceComment_PreservesExistingComment()
        {
            ProductKeyEntity existingEntity = CreateTestEntity(
                "DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant",
                comment: "Original comment.");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Comment = "   "
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Comment == "Original comment."));
        }

        [Test]
        public void UpdateProductKey_WithUnknownStatus_PreservesExistingStatus()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Unknown"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void UpdateProductKey_WithNullStatus_PreservesExistingStatus()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = null
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity => entity.Status == "Vacant"));
        }

        [Test]
        public void UpdateProductKey_WithValidRequest_CallsRepositoryUpdate()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Any<ProductKeyEntity>());
        }

        [Test]
        public void UpdateProductKey_WithValidRequest_CallsRepositorySaveChanges()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
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
        public void UpdateProductKey_WithValidRequest_SetsUpdatedDateTimeToARecentValue()
        {
            ProductKeyEntity existingEntity = CreateTestEntity("DARK-SOUL-S613-MNOP", "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new()
            {
                Key = "DARK-SOUL-S613-MNOP",
                Status = "Used"
            };
            DateTime beforeUpdate = DateTime.Now.AddSeconds(-1);

            service.UpdateProductKey(request);

            repository.Received(1).Update(Arg.Is<ProductKeyEntity>(entity =>
                DateTime.ParseExact(entity.UpdatedDateTime, DateTimeFormat, CultureInfo.InvariantCulture) >= beforeUpdate));
        }

        [Test]
        public void UpdateProductKey_WithValidRequest_UsesKeyToLookUpEntityById()
        {
            string key = "DARK-SOUL-S613-MNOP";
            string expectedId = ComputeKeyId(key);
            ProductKeyEntity existingEntity = CreateTestEntity(key, "NucilandiaSteam", "DarkSouls", "Vacant");
            repository.Get(Arg.Any<string>()).Returns(existingEntity);

            UpdateProductKeyRequest request = new() { Key = key, Status = "Used" };

            service.UpdateProductKey(request);

            repository.Received(1).Get(expectedId);
        }

        static ProductKeyEntity CreateTestEntity(
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

        static AddProductKeyRequest CreateAddRequest(
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

        static string ComputeKeyId(string key)
            => new Guid(MD5.HashData(Encoding.Default.GetBytes(key))).ToString();
    }
}
