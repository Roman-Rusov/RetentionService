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
                () => executor.ExecuteStorageCleanup(null, A.Fake<IStaleItemsDetector>()));
        }

        [Test]
        public void ExecuteStorageCleanup_should_throw_ArgumentNullException_if_stale_items_detector_argument_is_null()
        {
            var executor = CreateCleanupExecutor();

            Assert.ThrowsAsync<ArgumentNullException>(
                () => executor.ExecuteStorageCleanup(A.Fake<IResourceStorage>(), null));
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
                "CleanupExecutor absolutely relies on stale items detector even if the detector lies.")]
        [TestCase(
            "1 2 3",
            "100 500 -0.0001",
            Description =
                "CleanupExecutor absolutely relies on stale items detector even if the detector lies.")]
        public async Task ExecuteStorageCleanup_should_request_deletion_of_resources_considered_as_stale_by_stale_items_detector(
            string resourceDetails,
            string staleResourcesAddresses)
        {
            // Arrange.
            var staleResources = staleResourcesAddresses.Split(new[] {' '}, RemoveEmptyEntries);

            var resources = resourceDetails.ParseResourceDetails();
            var storage = CreateFakeStorage(resources);

            var staleItemsDetector = A.Fake<IStaleItemsDetector>();

            A.CallTo(() => staleItemsDetector.FindStaleItems(A<IEnumerable<(string, TimeSpan)>>._))
                .Returns(staleResources);

            var cleanupExecutor = CreateCleanupExecutor();

            // Act.
            await cleanupExecutor.ExecuteStorageCleanup(storage, staleItemsDetector);

            // Assert.
            _deletedResources.ShouldAllBeEquivalentTo(staleResources);
        }

        private IResourceStorage CreateFakeStorage(IEnumerable<ResourceDetails> resources)
        {
            var storage = A.Fake<IResourceStorage>();

            A.CallTo(() => storage.GetResourceDetails())
                .ReturnsLazily(() => Task.FromResult(resources));

            A.CallTo(() => storage.DeleteResources(A<IEnumerable<string>>._))
                .Invokes((IEnumerable<string> resourcesToDelete) =>
                {
                    _deletedResources = resourcesToDelete.ToArray();
                })
                .ReturnsLazily(() => Task.CompletedTask);

            return storage;
        }

        private static CleanupExecutor CreateCleanupExecutor()
            => new CleanupExecutor();

        private string[] _deletedResources;
    }
}