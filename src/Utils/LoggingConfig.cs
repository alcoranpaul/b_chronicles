using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Serilog;

namespace Utils;

public static class LoggingConfig
{
    public static ILoggerFactory? _loggerFactory;

    public static ServiceProvider ConfigureLogging(bool enableFileLogging = true)
    {
        // Ensure logs directory exists
        if (enableFileLogging)
        {
            Directory.CreateDirectory("logs");
        }

        // Use safe filename without colons
        string safeTimestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");

        // Configure Serilog
        LoggerConfiguration? loggerConfig = new LoggerConfiguration();

        if (enableFileLogging)
        {
            // Add file logging if enabled
            loggerConfig.WriteTo.File(
                $"logs/log_{safeTimestamp}.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            );
        }

        Log.Logger = loggerConfig.CreateLogger();

        ServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging(builder =>
        {
            builder.ClearProviders();

            // Add Serilog for logging
            builder.AddSerilog(dispose: true);

            // Configure console logging with custom formatter
            builder.AddConsole(options =>
            {
                options.FormatterName = "CustomFormatter";
            })
            .AddConsoleFormatter<CustomFormatter, ConsoleFormatterOptions>(options =>
            {
                options.TimestampFormat = "HH:mm:ss ";
                options.IncludeScopes = false;
            });
        });

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        _loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        return serviceProvider;
    }

    public static ILogger<T> GetLogger<T>()
    {
        if (_loggerFactory == null)
        {
            throw new InvalidOperationException("LoggerFactory is not initialized. Call ConfigureLogging() first.");
        }
        return _loggerFactory.CreateLogger<T>();
    }
}

public sealed class CustomFormatter : ConsoleFormatter, IDisposable
{
    private readonly IDisposable? _optionsReloadToken;
    private ConsoleFormatterOptions _formatterOptions;

    public CustomFormatter(IOptionsMonitor<ConsoleFormatterOptions> options)
        : base("CustomFormatter")
    {
        _optionsReloadToken = options.OnChange(ReloadLoggerOptions);
        _formatterOptions = options.CurrentValue;
    }

    private void ReloadLoggerOptions(ConsoleFormatterOptions options)
    {
        _formatterOptions = options;
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        string? message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message == null) return;

        // Get formatted timestamp
        string timestamp = DateTime.Now.ToString(_formatterOptions.TimestampFormat ?? "HH:mm:ss");

        // Get colored log level
        string logLevel = GetColoredLogLevel(logEntry.LogLevel);

        // Get caller information
        var callerInfo = GetCallerInfo();

        // Format: [HH:mm:ss] [info] (File.cs:123) message
        textWriter.WriteLine($"{timestamp}{logLevel} {message}");

        if (logEntry.Exception != null)
        {
            textWriter.WriteLine($"\n{logEntry.Exception}");
        }
    }

    private (string File, int Line) GetCallerInfo()
    {
        var stackTrace = new StackTrace(true);
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame?.GetMethod();
            var declaringType = method?.DeclaringType;

            // Skip logging framework and utility classes
            if (declaringType == null ||
                declaringType.FullName?.Contains("Microsoft.Extensions.Logging") == true ||
                declaringType.FullName?.Contains("Utils.LoggingConfig") == true ||
                declaringType.FullName?.Contains("Utils.ConsoleHelper") == true)
            {
                continue;
            }

            var file = Path.GetFileName(frame!.GetFileName()) ?? "unknown";
            return (file, frame.GetFileLineNumber());
        }
        return ("unknown", 0);
    }

    private string GetColoredLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Information => "\u001b[32m[info]\u001b[0m", // Green
            LogLevel.Warning => "\u001b[33m[warn]\u001b[0m",     // Yellow
            LogLevel.Error => "\u001b[31m[error]\u001b[0m",      // Red
            LogLevel.Critical => "\u001b[35m[critical]\u001b[0m",// Magenta
            LogLevel.Debug => "\u001b[36m[debug]\u001b[0m",      // Cyan
            LogLevel.Trace => "\u001b[37m[trace]\u001b[0m",      // White
            _ => "[Unknown]"                                     // Default
        };
    }

    public void Dispose()
    {
        _optionsReloadToken?.Dispose();
    }
}