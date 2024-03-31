using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Build.Framework.ILogger;

namespace Belp.Build.Test.MSBuild.Loggers;

/// <summary>
/// Provides an adapter for <see cref="ITestOutputHelper"/> to <see cref="ILogger"/>.
/// </summary>
public class XUnitLogger : ITestOutputHelper, ILogger
{
    private readonly MSBuildDiagnosticLogger _logger;
    private readonly ITestOutputHelper _output;
    private IEventSource _eventSource = null!;

    string? ILogger.Parameters
    {
        get => null;

        set
        {
        }
    }

    LoggerVerbosity ILogger.Verbosity
    {
        get => Verbosity.ToLoggerVerbosity();

        set => Verbosity = value.ToLogLevel();
    }

    /// <summary>
    /// Gets or sets the minimum level of logged messages.
    /// </summary>
    public LogLevel Verbosity { get; set; } = LogLevel.Information;



    /// <inheritdoc cref="MSBuildDiagnosticLogger.OnError" />
    public event DiagnosticEventHandler? OnError
    {
        add => _logger.OnError += value;
        remove => _logger.OnError -= value;
    }

    /// <inheritdoc cref="MSBuildDiagnosticLogger.OnWarning" />
    public event DiagnosticEventHandler? OnWarning
    {
        add => _logger.OnWarning += value;
        remove => _logger.OnWarning -= value;
    }

    /// <inheritdoc cref="MSBuildDiagnosticLogger.OnMessage" />
    public event DiagnosticEventHandler? OnMessage
    {
        add => _logger.OnMessage += value;
        remove => _logger.OnMessage -= value;
    }

    /// <inheritdoc cref="MSBuildDiagnosticLogger.OnDiagnostic" />
    public event DiagnosticEventHandler? OnDiagnostic
    {
        add => _logger.OnDiagnostic += value;
        remove => _logger.OnDiagnostic -= value;
    }



    /// <summary>
    /// Initializes a new instance of <see cref="XUnitLogger"/> for the specified <paramref name="output"/>.
    /// </summary>
    /// <param name="output">The <see cref="ITestOutputHelper"/> to adapt.</param>
    public XUnitLogger(ITestOutputHelper output)
    {
        _logger = new MSBuildDiagnosticLogger();
        _output = output;
    }

    #region ILogger

    /// <inheritdoc />
    public virtual void Initialize(IEventSource eventSource)
    {
        _eventSource = eventSource;

        eventSource.BuildFinished += OnBuildStatus;
        eventSource.BuildStarted += OnBuildStatus;
        eventSource.ProjectFinished += OnBuildStatus;
        eventSource.ProjectStarted += OnBuildStatus;

        OnError += OnErrorRaised;
        OnWarning += OnWarningRaised;
        OnMessage += OnMessageRaised;
    }

    /// <inheritdoc />
    public virtual void Shutdown()
    {
        _eventSource.BuildFinished -= OnBuildStatus;
        _eventSource.BuildStarted -= OnBuildStatus;
        _eventSource.ProjectFinished -= OnBuildStatus;
        _eventSource.ProjectStarted -= OnBuildStatus;

        OnError -= OnErrorRaised;
        OnWarning -= OnWarningRaised;
        OnMessage -= OnMessageRaised;
    }

    /// <summary>
    /// Runs when any status update is raised.
    /// </summary>
    /// <param name="sender">The status update's sender.</param>
    /// <param name="e">The raised status update.</param>
    protected virtual void OnBuildStatus(object sender, BuildStatusEventArgs e)
    {
        if (e.Message is not null)
        {
            WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Runs when an error is raised.
    /// </summary>
    /// <param name="diagnostic">The diagnostic that was captured.</param>
    protected virtual void OnErrorRaised(Diagnostic diagnostic)
    {
        if (diagnostic.Severity >= Verbosity)
        {
            WriteLine(diagnostic.ToString());
        }
    }

    /// <summary>
    /// Runs when a warning is raised.
    /// </summary>
    /// <param name="diagnostic">The diagnostic that was captured.</param>
    protected virtual void OnWarningRaised(Diagnostic diagnostic)
    {
        if (diagnostic.Severity >= Verbosity)
        {
            WriteLine(diagnostic.ToString());
        }
    }

    /// <summary>
    /// Runs when a message is raised.
    /// </summary>
    /// <param name="diagnostic">The diagnostic that was captured.</param>
    /// <exception cref="NotSupportedException">The message has an unsupported importance level.</exception>
    protected virtual void OnMessageRaised(Diagnostic diagnostic)
    {
        if (diagnostic.Severity >= Verbosity)
        {
            WriteLine(diagnostic.ToString());
        }
    }

    #endregion

    /// <inheritdoc />
    public void WriteLine(string message)
    {
        _output.WriteLine(message);
    }

    /// <inheritdoc />
    public void WriteLine(string format, params object[] args)
    {
        _output.WriteLine(format, args);
    }
}
