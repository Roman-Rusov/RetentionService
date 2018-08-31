using System;

using Common;

namespace Logging
{
    /// <summary>
    /// Represents wrapper for implementation of the <see cref="T:log4net.ILog"/> interface.
    /// </summary>
    public class Log4NetWrapper : ILog
    {
        private readonly log4net.ILog _logImpl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetWrapper" /> class.
        /// </summary>
        /// <param name="logImpl">
        /// The implementation of the <see cref="T:log4net.ILog"/> interface.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logImpl"/> is <see langword="null"/>.
        /// </exception>
        public Log4NetWrapper(log4net.ILog logImpl) => _logImpl = logImpl;

        /// <inheritdoc/>
        public void Debug(string message) => _logImpl.Debug(message);

        /// <inheritdoc/>
        public void Info(string message) => _logImpl.Info(message);

        /// <inheritdoc/>
        public void Warn(string message) => _logImpl.Warn(message);

        /// <inheritdoc/>
        public void Warn(string message, Exception exception) => _logImpl.Warn(message, exception);

        /// <inheritdoc/>
        public void Error(string message) => _logImpl.Error(message);

        /// <inheritdoc/>
        public void Error(string message, Exception exception) => _logImpl.Error(message, exception);

        /// <inheritdoc/>
        public void Fatal(string message) => _logImpl.Fatal(message);

        /// <inheritdoc/>
        public void Fatal(string message, Exception exception) => _logImpl.Fatal(message, exception);
    }
}