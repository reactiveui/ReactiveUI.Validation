// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Markup.Xaml;
using LoginApp.Avalonia.Services;
using LoginApp.Avalonia.Views;
using LoginApp.ViewModels;

namespace LoginApp.Avalonia;

/// <summary>
/// Defines the main Avalonia application class.
/// </summary>
/// <inheritdoc />
public class App : Application
{
    /// <inheritdoc />
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        new SignUpView { DataContext = new SignUpViewModel(null, new AvaloniaUserDialogs()) }.Show();
        base.OnFrameworkInitializationCompleted();
    }
}
