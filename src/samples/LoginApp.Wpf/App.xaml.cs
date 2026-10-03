// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace LoginApp.Wpf;

/// <summary>Defines the main Windows Presentation Framework application class.</summary>
/// <inheritdoc />
[System.Diagnostics.DebuggerDisplay("App: {ToString(),nq}")]
public partial class App : Application
{
    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App() => InitializeComponent();

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        AppBootstrapper.Create().CreateMainWindow().Show();
        base.OnStartup(e);
    }
}
