using Coyote.Console.Common.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Coyote.Console.Common.Services
{
    /// <summary>
    /// Handle logging information
    /// </summary>
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger _logger;
        public LoggerManager(ILogger<LoggerManager> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Logging Debug Function
        /// </summary>
        /// <param name="message">Message</param>
        public void LogDebug(string message)
        {
            _logger?.LogDebug(message);
        }
        /// <summary>
        /// Logging Error Function
        /// </summary>
        /// <param name="message">Message</param>
        public void LogError(string message, Exception ex = null)
        {
            if (ex == null)
                _logger?.LogError(message);
            else
                _logger?.LogError(ex, message);
        }
        /// <summary>
        /// Logging Information Function
        /// </summary>
        /// <param name="message">Message</param>
        public void LogInformation(string message)
        {
            _logger?.LogInformation(message);
        }
        /// <summary>
        /// Logging Warning Function
        /// </summary>
        /// <param name="message">Message</param>
        public void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }

        /// <summary>
        ///  WriteErrorLog   
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool WriteErrorLog(Exception ex)

        {
            bool Status = false;

            string LogDirectory = Path.Combine("Resources", "ErrorLog");

            DateTime CurrentDateTime = DateTime.Now;
            string CurrentDateTimeString = CurrentDateTime.ToString();
            CheckCreateLogDirectory(LogDirectory);
            string logLine = BuildLogLine(CurrentDateTime, ex);
            LogDirectory = (LogDirectory + "Log_" + LogFileName(DateTime.Now) + ".txt");

            StreamWriter oStreamWriter = null;
            try
            {
                oStreamWriter = new StreamWriter(LogDirectory, true);
                oStreamWriter.WriteLine(logLine);
                Status = true;
            }
            catch (Exception exp)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), exp);
            }
            finally
            {
                if (oStreamWriter != null)
                {
                    oStreamWriter.Close();
                }
            }
            return Status;
        }
        /// <summary>
        /// LogFileEntryDateTime
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static string LogFileEntryDateTime(DateTime CurrentDateTime)
        {
            return CurrentDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }
        /// <summary>
        /// LogFileName
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static string LogFileName(DateTime CurrentDateTime)
        {
            return CurrentDateTime.ToString("dd_MM_yyyy");
        }
        /// <summary>
        /// BuildLogLine
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string BuildLogLine(DateTime CurrentDateTime, Exception ex)
        {
            StringBuilder loglineStringBuilder = new StringBuilder();
            loglineStringBuilder.Append(LogFileEntryDateTime(CurrentDateTime));
            loglineStringBuilder.Append(" \t");
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            loglineStringBuilder.Append(message);
            return loglineStringBuilder.ToString();
        }
        /// <summary>
        /// CheckCreateLogDirectory
        /// </summary>
        /// <param name="LogPath"></param>
        /// <returns></returns>
        private static bool CheckCreateLogDirectory(string LogPath)
        {
            bool loggingDirectoryExists = false;
            DirectoryInfo oDirectoryInfo = new DirectoryInfo(LogPath);
            if (oDirectoryInfo.Exists)
            {
                loggingDirectoryExists = true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(LogPath);
                    loggingDirectoryExists = true;
                }
                catch(Exception exp)
                {
                    throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), exp);
                }
            }
            return loggingDirectoryExists;
        }
    }
}
