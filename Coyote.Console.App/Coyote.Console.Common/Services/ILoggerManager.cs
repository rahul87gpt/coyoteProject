using System;

namespace Coyote.Console.Common.Services
{
    /// <summary>
    /// Handle logging information
    /// </summary>
    public interface ILoggerManager
    {
        /// <summary>
        /// Logging Information Function
        /// </summary>
        /// <param name="message">Message</param>
        void LogInformation(string message);
        /// <summary>
        /// Logging Warning Function
        /// </summary>
        /// <param name="message">Message</param>
        void LogWarning(string message);
        /// <summary>
        /// Logging Debug Function
        /// </summary>
        /// <param name="message">Message</param>
        void LogDebug(string message);
        /// <summary>
        /// Logging Error Function
        /// </summary>
        /// <param name="message">Message</param>
        void LogError(string message, Exception ex = null);

        /// <summary>
        /// WriteErrorLog(string LogMessage);
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool WriteErrorLog(Exception ex);
    }
}
