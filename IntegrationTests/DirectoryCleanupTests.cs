﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Common;
using FluentAssertions;
using NUnit.Framework;

using RetentionService.Cleanup;
using RetentionService.FileSystemStorage;
using RetentionService.RetentionRules;
using RetentionService.Tests.Common;

using static System.StringSplitOptions;

namespace RetentionService.IntegrationTests
{
    /// <summary>
    /// Represents the set of integration tests involving the <see cref="CleanupExecutor"/>
    /// and <see cref="DirectoryFileStorage"/> classes.
    /// </summary>
    public class DirectoryCleanupTests
    {
        private const string TempFolderName = "RetentionService - DirectoryCleanupTests";
        private const string FileExtension = "backup";

        private string _storageDirectory;

        [SetUp]
        public void SetUp()
        {
            // TODO:  Use RAM drive.
            _storageDirectory = Path.Combine(Path.GetTempPath(), TempFolderName);

            if (Directory.Exists(_storageDirectory))
                Directory.Delete(_storageDirectory, recursive: true);

            Directory.CreateDirectory(_storageDirectory);
        }

        [TearDown]
        public void TearDown() =>
            Directory.Delete(_storageDirectory, recursive: true);

        [TestCase(
            "1:1",
            "0.2 0.9 1.1 2 3",
            "0.2 0.9 1.1")]
        [TestCase(
            "1:5 3:3 5:2 10:1 14:0",
            "0.2 0.9 14.1 15 22",
            "0.2 0.9")]
        [TestCase(
            "1:5 3:3 5:2 10:1 14:0",
            "0.2 0.9 1.1 2 3 4 5 6",
            "0.2 0.9 1.1 2 3 4 5")]
        public async Task Worker_should_cleanup_stale_files(
            string rulesData,
            string resourceDetails,
            string expectedRetainedFiles)
        {
            // Arrange.
            var expectedRetainedFileNames = expectedRetainedFiles.Split(new[] {' '}, RemoveEmptyEntries);

            var now = DateTime.UtcNow;

            var rules = rulesData.ParseRules();
            var ruleSet = new RetentionRuleSet(rules);

            var resources = resourceDetails.ParseResourceDetails();

            resources.ForEach(i =>
            {
                var resourceAddress = Path.Combine(_storageDirectory, $"{i.Address}.{FileExtension}");
                File.AppendAllText(resourceAddress, string.Empty); // Create empty file.
                File.SetLastWriteTimeUtc(resourceAddress, now - i.Age);
            });

            var storage = new DirectoryFileStorage(_storageDirectory);

            // Act.
            await new CleanupExecutor().ExecuteStorageCleanup(storage, ruleSet);

            // Assert.
            var actualRetainedFileNames = Directory
                .GetFiles(_storageDirectory)
                .Select(Path.GetFileNameWithoutExtension);

            actualRetainedFileNames.ShouldAllBeEquivalentTo(expectedRetainedFileNames);
        }
    }
}