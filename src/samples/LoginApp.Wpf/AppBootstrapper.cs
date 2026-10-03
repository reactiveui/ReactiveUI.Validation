// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows;
using LoginApp.Services;
using LoginApp.ViewModels;
using LoginApp.Wpf.Services;
using LoginApp.Wpf.Views;
using MahApps.Metro.Controls;
using ReactiveUI;
using Splat;

namespace LoginApp.Wpf;

/// <summary>
/// The app bootstrapper which is used to register everything with the Splat service locator.
/// It is also the central location for the RoutingState used for routing between views.
/// </summary>
[System.Diagnostics.DebuggerDisplay("AppBootstrapper: {Router}")]
public class AppBootstrapper : ReactiveObject, IScreen
{
    /// <summary>The width and height of the main window.</summary>
    private const int MainWindowSize = 400;

    /// <summary>Initializes a new instance of the <see cref="AppBootstrapper"/> class.</summary>
    private AppBootstrapper()
    {
    }

    /// <summary>Gets the router which is used to navigate between views.</summary>
    public RoutingState Router { get; } = new();

    /// <summary>Creates the bootstrapper, registers the dependencies and shows the sample page.</summary>
    /// <returns>The bootstrapper.</returns>
    public static AppBootstrapper Create()
    {
        var bootstrapper = new AppBootstrapper();

        // Register the dependencies.
        Locator.CurrentMutable.RegisterConstant(bootstrapper, typeof(IScreen));
        Locator.CurrentMutable.Register(static () => new SignUpView(), typeof(IViewFor<SignUpViewModel>));
        Locator.CurrentMutable.Register(static () => new WindowsUserDialogs(), typeof(IUserDialogs));

        // Show the sample page.
        _ = bootstrapper.Router
            .NavigateAndReset
            .Execute(new SignUpViewModel())
            .Subscribe();

        return bootstrapper;
    }

    /// <summary>Creates the first main page used within the application.</summary>
    /// <returns>The page generated.</returns>
    public Window CreateMainWindow() =>
        // NB: This returns the opening page that the platform-specific
        // boilerplate code will look for. It will know to find us because
        // we've registered our AppBootstrapScreen.
        new MetroWindow { Width = MainWindowSize, Height = MainWindowSize, Content = new RoutedViewHost { Router = Router } };
}
