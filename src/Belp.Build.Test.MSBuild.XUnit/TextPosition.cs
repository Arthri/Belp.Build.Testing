#nullable enable
#if !BELP_BUILD_TEST_MSBUILD_XUNIT_ENABLE_WARNINGS
#pragma warning disable
#endif

namespace Belp.Build.Test.MSBuild.XUnit;

internal record struct TextPosition(int Line, int Column);
