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
    /// <typeparam name="T">
    /// The type of an identifier of a resource.
    /// </typeparam>
    public class App<T> : IApp
    {
        private readonly CleanupExecutor _cleanupExecutor;
        private readonly IResourceStorage<T> _storage;
        private readonly IResourceExpirationPolicy _expirationPolicy;
        [NotNull] private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="App{T}" /> class.
        /// </summary>
        /// <param name="cleanupExecutor">
        /// The executor of cleanup of the <paramref name="storage"/>.
        /// </param>
        /// <param name="storage">
        /// The storage to be cleanup up.
        /// </param>
        /// <param name="expirationPolicy">
        /// The resource expiration policy.
        /// </param>
        /// <param name="log">
        /// The log where to write messages to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="cleanupExecutor"/> is <see langword="null"/> or
        /// <paramref name="storage"/> is <see langword="null"/> or
        /// <paramref name="expirationPolicy"/> is <see langword="null"/> or
        /// <paramref name="log"/> is <see langword="null"/>.
        /// </exception>
        public App(
            [NotNull] CleanupExecutor cleanupExecutor,
            [NotNull] IResourceStorage<T> storage,
            [NotNull] IResourceExpirationPolicy expirationPolicy,
            [NotNull] ILog log)
        {
            AssertArg.NotNull(cleanupExecutor, nameof(cleanupExecutor));
            AssertArg.NotNull(storage, nameof(storage));
            AssertArg.NotNull(expirationPolicy, nameof(expirationPolicy));
            AssertArg.NotNull(log, nameof(log));

            _cleanupExecutor = cleanupExecutor;
            _storage = storage;
            _expirationPolicy = expirationPolicy;
            _log = log;
        }

        /// <summary>
        /// Runs the application.
        /// </summary>
        public async Task Run()
        {
            try
            {
                await _cleanupExecutor.ExecuteStorageCleanup(_storage, _expirationPolicy);
            }
            catch (Exception ex)
            {
                _log.Error("An error occurred.", ex);
            }
        }
    }
}
