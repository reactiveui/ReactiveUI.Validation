// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

using Avalonia;

namespace AvaloniaApplication1.Desktop;

/// <summary>The entry point of the desktop application.</summary>
internal static class Program
{
    /// <summary>Starts the application.</summary>
    /// <param name="args">The command line arguments.</param>
    /// <remarks>
    /// Do not use any Avalonia, third-party APIs or any SynchronizationContext-reliant code before AppMain is called.
    /// They are not initialized yet.
    /// </remarks>
    [STAThread]
    internal static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>Builds the Avalonia configuration. The visual designer also uses it, so do not remove it.</summary>
    /// <returns>The application builder.</returns>
    internal static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
