using System;

namespace Common
{
    /// <summary>
    /// Represents a set of methods to write messages into a log.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message"> The message to log. </param>
        void Debug(string message);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message"> The message to log. </param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message"> The message to log. </param>
        void Warn(string message);

        /// <summary>
        /// Logs a warning message including details of the <paramref name="exception"/> specified.
        /// </summary>
        /// <param name="message"> The message to log. </param>
        /// <param name="exception"> The exception which details to log. </param>
        void Warn(string message, Exception exception);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Error(string message);

        /// <summary>
        /// Logs an error message including details of the <paramref name="exception"/> specified.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception"> The exception which details to log. </param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Fatal(string message);

        /// <summary>
        /// Logs a fatal message including details of the <paramref name="exception"/> specified.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception"> The exception which details to log. </param>
        void Fatal(string message, Exception exception);
    }
}