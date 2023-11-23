﻿using Microsoft.Build.Locator;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Contains the module initialization.
/// </summary>
file static class Initializer
{
    /// <summary>
    /// Invokes <see cref="MSBuildLocator"/> to add/link the MSBuild assemblies to the runtime.
    /// </summary>
    /// <exception cref="InvalidOperationException">A suitable version of .NET SDK was not found.</exception>
#pragma warning disable CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    [ModuleInitializer]
#pragma warning restore CA2255 // The 'ModuleInitializer' attribute should not be used in libraries
    internal static void InitializeModule()
    {
        if (MSBuildLocator.IsRegistered)
        {
            return;
        }

        VisualStudioInstance? latestInstance = MSBuildLocator
            .QueryVisualStudioInstances()
            .Where(static i => i.DiscoveryType == DiscoveryType.DotNetSdk)
            .OrderByDescending(static i => i.Version)
            .FirstOrDefault()
            ?? throw new InvalidOperationException(".NET SDK not found")
            ;

        if (latestInstance.Version.Major < 5)
        {
            throw new InvalidOperationException(".NET SDK 5 or higher not found");
        }

        MSBuildLocator.RegisterInstance(latestInstance);
    }
}
