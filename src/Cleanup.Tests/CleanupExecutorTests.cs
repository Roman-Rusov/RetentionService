using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;

using RetentionService.Cleanup.Contracts;
using RetentionService.Tests.Common;

using static System.StringSplitOptions;

namespace RetentionService.Cleanup.Tests
{
    /// <summary>
    /// Represents the set of tests of the <see cref="CleanupExecutor"/> class.
    /// </summary>
    public class CleanupExecutorTests
    {
        [Test]
        public void ExecuteStorageCleanup_should_throw_ArgumentNullException_if_storage_argument_is_null()
        {
            var executor = CreateCleanupExecutor();

            Assert.ThrowsAsync<ArgumentNullException>(
                () => executor.ExecuteStorageCleanup<object>(null, A.Fake<IResourceExpirationPolicy>()));
        }

        [Test]
        public void ExecuteStorageCleanup_should_throw_ArgumentNullException_if_expirationPolicy_argument_is_null()
        {
            var executor = CreateCleanupExecutor();

            Assert.ThrowsAsync<ArgumentNullException>(
                () => executor.ExecuteStorageCleanup(A.Fake<IResourceStorage<object>>(), null));
        }

        [TestCase(
            "0.2 0.9 1.1 2 3",
            "3")]
        [TestCase(
            "0.2 0.9 14.1 15 22",
            "14.1 15 22")]
        [TestCase(
            "0.2 0.9 1.1 2 3 4 5 6",
            "0.9 1.1 2 3 4 5 6")]
        [TestCase(
            "0.2 0.9 1.1 2 3 4 5 6",
            "0.9 1.1 2 3 4 5 6")]
        [TestCase(
            "0.2 0.9 1.1 2 3 4 5 6",
            "0.2 0.9 1.1 2 3 4 5 6")]
        [TestCase(
            "0.2 0.9 1.1 2 3 4 5 6",
            "0.2 1.1 3 5",
            Description =
                "CleanupExecutor absolutely relies on expiration policy even if the policy lies.")]
        [TestCase(
            "1 2 3",
            "100 500 -0.0001",
            Description =
                "CleanupExecutor absolutely relies on expiration policy even if the policy lies.")]
        public async Task ExecuteStorageCleanup_should_request_deletion_of_resources_considered_as_expired_by_expirationPolicy(
            string resourceDetails,
            string expiredResourcesIdsData)
        {
            // Arrange.
            var expiredResourceIds = expiredResourcesIdsData.Split(new[] {' '}, RemoveEmptyEntries);

            var resources = resourceDetails.ParseResources();
            var storage = CreateFakeStorage(resources);

            var expirationPolicy = A.Fake<IResourceExpirationPolicy>();

            A.CallTo(() => expirationPolicy.FindExpiredResources(A<IReadOnlyCollection<IResource<string>>>._))
                .Returns(expiredResourceIds);

            var cleanupExecutor = CreateCleanupExecutor();

            // Act.
            await cleanupExecutor.ExecuteStorageCleanup(storage, expirationPolicy);

            // Assert.
            _deletedResourceIds.ShouldAllBeEquivalentTo(expiredResourceIds);
        }

        private IResourceStorage<string> CreateFakeStorage(IReadOnlyCollection<FakeResource> resources)
        {
            var storage = A.Fake<IResourceStorage<string>>();

            A.CallTo(() => storage.GetResourceDetails())
                .ReturnsLazily(() => Task.FromResult(resources.Cast<IResource<string>>()));

            A.CallTo(() => storage.DeleteResources(A<IEnumerable<string>>._))
                .Invokes((IEnumerable<string> resourceIdsToDelete) =>
                {
                    _deletedResourceIds = resourceIdsToDelete.ToArray();
                })
                .ReturnsLazily(() => Task.CompletedTask);

            return storage;
        }

        private static CleanupExecutor CreateCleanupExecutor()
            => new CleanupExecutor();

        private string[] _deletedResourceIds;
    }
}