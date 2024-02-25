using Microsoft.Build.Framework;
using System.Collections;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Provides an adapter for <see cref="ITestOutputHelper"/> to <see cref="ILogger"/>.
/// </summary>
public class XUnitMSBuildLoggerAdapter : ITestOutputHelper, ILogger
{
    private sealed class DiagnosticAggregateList : IReadOnlyList<Diagnostic>
    {
        private readonly IReadOnlyList<Diagnostic> _errors;
        private readonly IReadOnlyList<Diagnostic> _warnings;
        private readonly IReadOnlyList<Diagnostic> _messages;

        public Diagnostic this[int index]
        {
            get
            {
                return index < _errors.Count
                    ? _errors[index]
                    : index - _errors.Count < _warnings.Count
                        ? _warnings[index]
                        : _messages[index]
                    ;
            }
        }

        public int Count => _errors.Count + _warnings.Count + _messages.Count;

        public DiagnosticAggregateList(XUnitMSBuildLoggerAdapter logger)
        {
            _errors = logger._errors;
            _warnings = logger._warnings;
            _messages = logger._messages;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Diagnostic> GetEnumerator()
        {
            for (int i = 0; i < _errors.Count; i++)
            {
                yield return _errors[i];
            }

            for (int i = 0; i < _warnings.Count; i++)
            {
                yield return _warnings[i];
            }

            for (int i = 0; i < _messages.Count; i++)
            {
                yield return _messages[i];
            }
        }
    }

    private readonly ITestOutputHelper _output;
    private IEventSource _eventSource;

    string? ILogger.Parameters
    {
        get => null;

        set
        {
        }
    }

    /// <inheritdoc />
    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Normal;



    private readonly List<Diagnostic> _errors = [];

    /// <summary>
    /// Gets a list of errors catched by the logger.
    /// </summary>
    public IReadOnlyList<Diagnostic> Errors => _errors.AsReadOnly();

    private readonly List<Diagnostic> _warnings = [];

    /// <summary>
    /// Gets a list of warnings catched by the logger.
    /// </summary>
    public IReadOnlyList<Diagnostic> Warnings => _warnings.AsReadOnly();

    private readonly List<Diagnostic> _messages = [];

    /// <summary>
    /// Gets a list of messages catched by the logger.
    /// </summary>
    public IReadOnlyList<Diagnostic> Messages => _messages.AsReadOnly();

    /// <summary>
    /// Gets a list of diagnostics(errors, warnings, and messages) catched by the logger.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics => new DiagnosticAggregateList(this);



    /// <summary>
    /// Initializes a new instance of <see cref="XUnitMSBuildLoggerAdapter"/> for the specified <paramref name="output"/>.
    /// </summary>
    /// <param name="output">The <see cref="ITestOutputHelper"/> to adapt.</param>
    public XUnitMSBuildLoggerAdapter(ITestOutputHelper output)
    {
        _output = output;
        _eventSource = null!;
    }

    #region ILogger

    void ILogger.Initialize(IEventSource eventSource)
    {
        Initialize(eventSource);
    }

    void ILogger.Shutdown()
    {
        Shutdown();
    }

    /// <inheritdoc cref="ILogger.Initialize(IEventSource)" />
    public virtual void Initialize(IEventSource eventSource)
    {
        _eventSource = eventSource;

        eventSource.BuildFinished += OnBuildStatus;
        eventSource.BuildStarted += OnBuildStatus;
        eventSource.ProjectFinished += OnBuildStatus;
        eventSource.ProjectStarted += OnBuildStatus;

        eventSource.ErrorRaised += OnErrorRaised;
        eventSource.WarningRaised += OnWarningRaised;
        eventSource.MessageRaised += OnMessageRaised;
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
    /// <param name="sender">The error's sender.</param>
    /// <param name="e">The raised error.</param>
    protected virtual void OnErrorRaised(object sender, BuildErrorEventArgs e)
    {
        var diagnostic = new Diagnostic(
            Diagnostic.SeverityLevel.Error,
            e.Code,
            e.Message,
            e.File,
            new TextSpan(
                new TextSpan.Position(e.LineNumber, e.ColumnNumber),
                new TextSpan.Position(e.EndLineNumber, e.EndColumnNumber)
            ),
            e.ProjectFile
        );
        _errors.Add(diagnostic);

        if (OnDiagnosticRaised(sender, diagnostic))
        {
            return;
        }

        if (Verbosity >= LoggerVerbosity.Quiet)
        {
            WriteLine(diagnostic.ToString());
        }
    }

    /// <summary>
    /// Runs when a warning is raised.
    /// </summary>
    /// <param name="sender">The warning's sender.</param>
    /// <param name="e">The raised warning.</param>
    protected virtual void OnWarningRaised(object sender, BuildWarningEventArgs e)
    {
        var diagnostic = new Diagnostic(
            Diagnostic.SeverityLevel.Warning,
            e.Code,
            e.Message,
            e.File,
            new TextSpan(
                new TextSpan.Position(e.LineNumber, e.ColumnNumber),
                new TextSpan.Position(e.EndLineNumber, e.EndColumnNumber)
            ),
            e.ProjectFile
        );
        _warnings.Add(diagnostic);

        if (OnDiagnosticRaised(sender, diagnostic))
        {
            return;
        }

        if (Verbosity >= LoggerVerbosity.Minimal)
        {
            WriteLine(diagnostic.ToString());
        }
    }

    /// <summary>
    /// Runs when a message is raised.
    /// </summary>
    /// <param name="sender">The message's sender.</param>
    /// <param name="e">The raised message.</param>
    /// <exception cref="NotSupportedException">The message has an unsupported importance level.</exception>
    protected virtual void OnMessageRaised(object sender, BuildMessageEventArgs e)
    {
        var diagnostic = new Diagnostic(
            e.Importance switch
            {
                MessageImportance.High => Diagnostic.SeverityLevel.Informational,
                MessageImportance.Normal => Diagnostic.SeverityLevel.Verbose,
                MessageImportance.Low => Diagnostic.SeverityLevel.Diagnostic,
                var importance => throw new NotSupportedException($"Unsupported importance level {importance}"),
            },
            e.Code,
            e.Message,
            e.File,
            new TextSpan(
                new TextSpan.Position(e.LineNumber, e.ColumnNumber),
                new TextSpan.Position(e.EndLineNumber, e.EndColumnNumber)
            ),
            e.ProjectFile
        );
        if (e.Importance <= MessageImportance.High)
        {
            _messages.Add(diagnostic);
        }

        if (OnDiagnosticRaised(sender, diagnostic))
        {
            return;
        }

        if (
            Verbosity switch
            {
                LoggerVerbosity.Detailed => e.Importance <= MessageImportance.Normal,
                LoggerVerbosity.Diagnostic => e.Importance <= MessageImportance.Low,
                LoggerVerbosity.Quiet or LoggerVerbosity.Minimal or LoggerVerbosity.Normal => e.Importance <= MessageImportance.High,
                _ => throw new NotSupportedException(),
            }
        )
        {
            WriteLine(diagnostic.ToString());
        }
    }

    /// <summary>
    /// Runs when a diagnostic of any severity is raised.
    /// </summary>
    /// <param name="sender">The diagnostic's sender.</param>
    /// <param name="diagnostic">The raised diagnostic.</param>
    /// <returns><see langword="true"/> if the processing of the diagnostic should terminate; otherwise, <see langword="false"/>.</returns>
    protected virtual bool OnDiagnosticRaised(object sender, Diagnostic diagnostic)
    {
        return false;
    }

    /// <inheritdoc cref="ILogger.Shutdown" />
    public virtual void Shutdown()
    {
        _eventSource.BuildFinished -= OnBuildStatus;
        _eventSource.BuildStarted -= OnBuildStatus;
        _eventSource.ProjectFinished -= OnBuildStatus;
        _eventSource.ProjectStarted -= OnBuildStatus;

        _eventSource.ErrorRaised -= OnErrorRaised;
        _eventSource.WarningRaised -= OnWarningRaised;
        _eventSource.MessageRaised -= OnMessageRaised;
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
