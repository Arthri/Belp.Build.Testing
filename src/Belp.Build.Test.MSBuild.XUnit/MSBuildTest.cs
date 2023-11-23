using Belp.Build.Test.MSBuild.XUnit.Resources;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Belp.Build.Test.MSBuild.XUnit;

/// <summary>
/// Provides the boilerplate for testing MSBuild projects.
/// </summary>
public class MSBuildTest
{
    public readonly ref struct TestDataFacade
    {
        public readonly ref struct TestProjectFacade
        {
            public readonly ref struct ProjectSourceFacade
            {
                private readonly ITestOutputHelper _logger;

                public ITestOutputHelper Logger => _logger;

                public TestProjectInstance Samples(string projectName, [CallerMemberName] string? callerMemberName = null)
                {
                    ArgumentException.ThrowIfNullOrEmpty(projectName);
                    ArgumentException.ThrowIfNullOrEmpty(callerMemberName);

                    return new TestProjectInstance(
                        callerMemberName,
                        TestProjectManager.TestProjects[projectName],
                        _logger
                    );
                }

                public ProjectSourceFacade(ITestOutputHelper logger)
                {
                    _logger = logger;
                }
            }

            private readonly ITestOutputHelper _logger;

            public ProjectSourceFacade From => new(_logger);

            public TestProjectFacade(ITestOutputHelper logger)
            {
                _logger = logger;
            }
        }

        private readonly ITestOutputHelper _logger;

        public TestProjectFacade Project => new(_logger);

        public TestDataFacade(ITestOutputHelper logger)
        {
            _logger = logger;
        }
    }

    public ITestOutputHelper Logger { get; private set; }

    public TestDataFacade Data => new(Logger);

    public MSBuildTest(ITestOutputHelper logger)
    {
        Logger = new XUnitMSBuildLoggerAdapter(logger);
    }

    public MSBuildTest(XUnitMSBuildLoggerAdapter logger)
    {
        Logger = logger;
    }

    public void SetLogger(XUnitMSBuildLoggerAdapter logger)
    {
        Logger = logger;
    }
}
