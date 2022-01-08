// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

namespace LoginApp.Wpf;

/// <summary>
/// Defines the main Windows Presentation Framework application class.
/// </summary>
/// <inheritdoc />
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App() => InitializeComponent();

    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        new AppBootstrapper().CreateMainWindow().Show();
        base.OnStartup(e);
    }
}
