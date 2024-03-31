using Belp.Build.Test.MSBuild.ObjectModel;
using Microsoft.Build.Framework;
using System.Collections;

namespace Belp.Build.Test.MSBuild.Loggers;

/// <summary>
/// Captures events from MSBuild as <see cref="Diagnostic"/>s.
/// </summary>
public sealed class MSBuildDiagnosticLogger : ILogger
{
    private sealed class DiagnosticAggregateList : IReadOnlyList<Diagnostic>
    {
        private readonly IReadOnlyList<Diagnostic> _errors;
        private readonly IReadOnlyList<Diagnostic> _warnings;

        public Diagnostic this[int index]
        {
            get
            {
                return index < _errors.Count
                    ? _errors[index]
                    : _warnings[index - _errors.Count]
                    ;
            }
        }

        public int Count => _errors.Count + _warnings.Count;

        public DiagnosticAggregateList(MSBuildDiagnosticLogger logger)
        {
            _errors = logger._errors;
            _warnings = logger._warnings;
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
        }
    }

    private IEventSource _eventSource = null!;

    /// <inheritdoc />
    public LoggerVerbosity Verbosity { get; set; } = LoggerVerbosity.Minimal;

    /// <inheritdoc />
    string? ILogger.Parameters
    {
        get => null;

        set
        {
        }
    }



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

    /// <summary>
    /// Gets a list of diagnostics(errors, warnings, and messages) catched by the logger.
    /// </summary>
    public IReadOnlyList<Diagnostic> Diagnostics => new DiagnosticAggregateList(this);

    /// <summary>
    /// Raised when an error is captured by the logger.
    /// </summary>
    public event DiagnosticEventHandler? OnError;

    /// <summary>
    /// Raised when a warning is captured by the logger.
    /// </summary>
    public event DiagnosticEventHandler? OnWarning;

    /// <summary>
    /// Raised when a message is captured by the logger.
    /// </summary>
    public event DiagnosticEventHandler? OnMessage;

    /// <summary>
    /// Raised when any diagnostic is captured by the logger.
    /// </summary>
    public event DiagnosticEventHandler? OnDiagnostic;



    /// <inheritdoc />
    public void Initialize(IEventSource eventSource)
    {
        _eventSource = eventSource;

        eventSource.ErrorRaised += OnErrorRaised;
        eventSource.WarningRaised += OnWarningRaised;
        eventSource.MessageRaised += OnMessageRaised;
    }

    /// <inheritdoc />
    public void Shutdown()
    {
        _eventSource.ErrorRaised -= OnErrorRaised;
        _eventSource.WarningRaised -= OnWarningRaised;
        _eventSource.MessageRaised -= OnMessageRaised;
    }



    /// <summary>
    /// Runs when an error is raised.
    /// </summary>
    /// <param name="sender">The error's sender.</param>
    /// <param name="e">The raised error.</param>
    private void OnErrorRaised(object sender, BuildErrorEventArgs e)
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

        OnError?.Invoke(diagnostic);
        OnDiagnostic?.Invoke(diagnostic);
    }

    /// <summary>
    /// Runs when a warning is raised.
    /// </summary>
    /// <param name="sender">The warning's sender.</param>
    /// <param name="e">The raised warning.</param>
    private void OnWarningRaised(object sender, BuildWarningEventArgs e)
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

        OnWarning?.Invoke(diagnostic);
        OnDiagnostic?.Invoke(diagnostic);
    }

    /// <summary>
    /// Runs when a message is raised.
    /// </summary>
    /// <param name="sender">The message's sender.</param>
    /// <param name="e">The raised message.</param>
    /// <exception cref="NotSupportedException">The message has an unsupported importance level.</exception>
    private void OnMessageRaised(object sender, BuildMessageEventArgs e)
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

        OnMessage?.Invoke(diagnostic);
        OnDiagnostic?.Invoke(diagnostic);
    }
}
