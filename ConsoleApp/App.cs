using System;
using System.Threading.Tasks;

using Common;
using JetBrains.Annotations;

using RetentionService.Cleanup;
using RetentionService.Cleanup.Contracts;

namespace RetentionService.ConsoleApp
{
    /// <summary>
    /// Represents the application.
    /// </summary>
    public class App
    {
        private readonly CleanupExecutor _cleanupExecutor;
        private readonly IResourceStorage _storage;
        private readonly IStaleItemsDetector _staleItemsDetector;
        [NotNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        /// <param name="cleanupExecutor">
        /// The executor of cleanup of the <paramref name="storage"/>.
        /// </param>
        /// <param name="storage">
        /// The storage to be cleanup up.
        /// </param>
        /// <param name="staleItemsDetector">
        /// The mean to detect stale resource in the <paramref name="storage"/>.
        /// </param>
        /// <param name="log">
        /// The log where to write messages to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cleanupExecutor"/> is <see langword="null"/> or
        /// <paramref name="storage"/> is <see langword="null"/> or
        /// <paramref name="staleItemsDetector"/> is <see langword="null"/> or
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public App(
            [NotNull] CleanupExecutor cleanupExecutor,
            [NotNull] IResourceStorage storage,
            [NotNull] IStaleItemsDetector staleItemsDetector,
            [NotNull] ILog log)
        {
            AssertArg.NotNull(cleanupExecutor, nameof(cleanupExecutor));
            AssertArg.NotNull(storage, nameof(storage));
            AssertArg.NotNull(staleItemsDetector, nameof(staleItemsDetector));
            AssertArg.NotNull(log, nameof(log));

            _cleanupExecutor = cleanupExecutor;
            _storage = storage;
            _staleItemsDetector = staleItemsDetector;
            _log = log;
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public async Task Run()
        {
            try
            {
                await _cleanupExecutor.ExecuteStorageCleanup(_storage, _staleItemsDetector);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred.", ex);
            }
        }
    }
}
