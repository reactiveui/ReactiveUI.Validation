// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LoginApp.Avalonia.Services;
using LoginApp.Avalonia.Views;
using LoginApp.ViewModels;

namespace LoginApp.Avalonia;

/// <summary>
/// App.
/// </summary>
/// <seealso cref="Avalonia.Application" />
public partial class App : Application
{
    /// <summary>
    /// Initializes the application by loading XAML etc.
    /// </summary>
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <summary>
    /// Called when [framework initialization completed].
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new SignUpViewModel(null, new AvaloniaUserDialogs())
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new SignUpView
            {
                DataContext = new SignUpViewModel(null, new AvaloniaUserDialogs())
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
