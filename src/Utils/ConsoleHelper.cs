using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Utils;

public static class ConsoleHelper
{

    /// <summary>
    /// Prints a message to the console.
    /// </summary>
    /// <param name="message"></param>
    private static void Print(string message)
    {
        Console.WriteLine(message);
    }

    /// <summary>
    /// Prints a message to the console, ensuring that it is not null or empty.
    /// </summary>
    /// <param name="message"></param>
    public static void Print(object message)
    {
        if (message != null && message.GetType() != typeof(string)) message = message.ToString() ?? string.Empty;

        if ((message as string) == string.Empty) return;

        if (message != null) Print(message);
    }

    public static void ClearConsole() { Console.Clear(); }

    public static void LogTrace(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Trace, message, filePath, lineNumber);
    }

    public static void LogDebug(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Debug, message, filePath, lineNumber);
    }

    public static void LogInfo(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Information, message, filePath, lineNumber);
    }

    public static void LogWarning(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Warning, message, filePath, lineNumber);
    }

    public static void LogError(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Error, message, filePath, lineNumber);
    }

    public static void LogCritical(
        string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Critical, message, filePath, lineNumber);
    }

    private static void Log(
        LogLevel logLevel,
        string message,
        string filePath,
        int lineNumber)
    {
        if (_loggerFactory != null)
        {
            CreateLoggerForCaller(filePath, out ILogger logger, out string fileName);
            logger.Log(logLevel, "[{file}:{line}] {message}", fileName, lineNumber, message);
        }
        else
        {
            Print($"[{Path.GetFileName(filePath)}:{lineNumber}] {message}");
        }
    }

    private static void CreateLoggerForCaller(string filePath, out ILogger logger, out string fileName)
    {
        string callingClassName = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType?.Name ?? "UnknownClass";
        if (LoggingConfig._loggerFactory == null)
        {
            throw new InvalidOperationException("LoggerFactory is not initialized.");
        }
        logger = LoggingConfig._loggerFactory.CreateLogger(callingClassName);

        // Include caller info in the log message
        fileName = Path.GetFileName(filePath);
    }
}