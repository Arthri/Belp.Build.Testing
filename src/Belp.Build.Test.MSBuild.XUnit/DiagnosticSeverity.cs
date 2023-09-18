#nullable enable
#if !BELP_BUILD_TEST_MSBUILD_XUNIT_ENABLE_WARNINGS
#pragma warning disable
#endif

namespace Belp.SDK.Test.MSBuild.XUnit;

internal enum DiagnosticSeverity
{
    Critical = 1,
    Error = 2,
    Warning = 3,
    Informational = 4,
    Verbose = 5,
    Diagnostic = 6,
}
