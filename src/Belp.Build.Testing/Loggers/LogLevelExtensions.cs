﻿using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace Belp.Build.Testing.Loggers;

/// <summary>
/// Provides extensions for <see cref="LogLevel"/>.
/// </summary>
internal static class LogLevelExtensions
{
    internal static LoggerVerbosity ToLoggerVerbosity(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LoggerVerbosity.Diagnostic,
            LogLevel.Debug => LoggerVerbosity.Detailed,
            LogLevel.Information => LoggerVerbosity.Normal,
            LogLevel.Warning => LoggerVerbosity.Minimal,
            LogLevel.Error => LoggerVerbosity.Minimal,
            LogLevel.Critical => LoggerVerbosity.Minimal,
            LogLevel.None => LoggerVerbosity.Quiet,
            var level => throw new NotSupportedException($"Unsupported log level {level}."),
        };
    }
}
