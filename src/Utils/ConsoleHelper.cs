using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Utils;

public static class ConsoleHelper
{
    /// <summary>
    /// Prints a message to the console.
    /// </summary>
    /// <param name="message"></param>
    public static void Print(string message)
    {
        Console.WriteLine(message);
    }

    public static void Print(string message, ConsoleColor color, bool newLine = true)
    {
        Console.ForegroundColor = color;
        if (newLine)
            Console.WriteLine(message);
        else
            Console.Write(message);
        Console.ResetColor();
    }



    /// <summary>
    /// Prints a message to the console, ensuring that it is not null or empty.
    /// </summary>
    /// <param name="message"></param>
    public static void Print(object message)
    {
        if (message != null && message.GetType() != typeof(string))
        {
            message = message.ToString() ?? string.Empty;
        }

        if ((message as string) == string.Empty) return;

        if (message != null) Print(message);
    }

    public static void ClearConsole() { Console.Clear(); }

    public static void LogTrace(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Trace, message, filePath, lineNumber);
    }

    public static void LogDebug(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Debug, message, filePath, lineNumber);
    }

    public static void LogInfo(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Information, message, filePath, lineNumber);
    }

    public static void LogWarning(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Warning, message, filePath, lineNumber);
    }

    public static void LogError(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Error, message, filePath, lineNumber);
        Environment.Exit(1);
    }

    public static void LogCritical(
        object message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        Log(LogLevel.Critical, message, filePath, lineNumber);
    }

    private static void Log(
        LogLevel logLevel,
        object message,
        string filePath,
        int lineNumber)
    {
        // Convert the object to a string (use JSON serialization for complex objects)
        string? messageString = message is string ? message.ToString() : JsonSerializer.Serialize(message);

        if (_loggerFactory != null)
        {
            CreateLoggerForCaller(filePath, out ILogger logger, out string fileName);
            logger.Log(logLevel, "[{file}:{line}] {message}", fileName, lineNumber, messageString);
        }
        else
        {
            Print($"[{Path.GetFileName(filePath)}:{lineNumber}] {messageString}");
        }
    }

    private static void CreateLoggerForCaller(string filePath, out ILogger logger, out string fileName)
    {
        string callingClassName = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType?.Name ?? "UnknownClass";
        if (_loggerFactory == null)
        {
            throw new InvalidOperationException("LoggerFactory is not initialized.");
        }
        logger = _loggerFactory.CreateLogger(callingClassName);

        // Include caller info in the log message
        fileName = Path.GetFileName(filePath);
    }
}