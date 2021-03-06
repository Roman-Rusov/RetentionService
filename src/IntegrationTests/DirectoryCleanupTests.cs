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

        private DirectoryFileStorageSettings _settings;

        [SetUp]
        public void SetUp()
        {
            // TODO:  Use RAM drive.
            var storageDirectory = Path.Combine(Path.GetTempPath(), TempFolderName);

            if (Directory.Exists(storageDirectory))
                Directory.Delete(storageDirectory, recursive: true);

            Directory.CreateDirectory(storageDirectory);

            _settings = new DirectoryFileStorageSettings(storageDirectory);
        }

        [TearDown]
        public void TearDown() =>
            Directory.Delete(_settings.DirectoryPath, recursive: true);

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
        public async Task CleanupExecutor_should_delete_expired_files(
            string rulesData,
            string resourceDetails,
            string expectedRetainedFiles)
        {
            // Arrange.
            var expectedRetainedFileNames = expectedRetainedFiles.Split(' ', RemoveEmptyEntries);

            var rules = rulesData.ParseRules();
            var policy = new RetentionPolicy(rules);

            CreateFilesInStorage(resourceDetails);

            var storage = new DirectoryFileStorage(new SystemClock(), _settings);

            // Act.
            await new CleanupExecutor().ExecuteStorageCleanup(storage, policy);

            // Assert.
            var actualRetainedFileNames = Directory
                .GetFiles(_settings.DirectoryPath)
                .Select(Path.GetFileNameWithoutExtension);

            actualRetainedFileNames.Should().BeEquivalentTo(expectedRetainedFileNames);
        }

        private void CreateFilesInStorage(string resourceDetails)
        {
            var now = DateTime.UtcNow;

            var resources = resourceDetails.ParseResources();

            resources.ForEach(r =>
            {
                var filePath = Path.Combine(_settings.DirectoryPath, $"{r.Id}.{FileExtension}");
                File.AppendAllText(filePath, string.Empty); // Create empty file.
                File.SetLastWriteTimeUtc(filePath, now - r.Age);
            });
        }
    }
}