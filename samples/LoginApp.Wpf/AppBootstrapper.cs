// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
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
public class AppBootstrapper : ReactiveObject, IScreen
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
    /// </summary>
    public AppBootstrapper()
    {
        // Register the dependencies.
        Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
        Locator.CurrentMutable.Register(() => new SignUpView(), typeof(IViewFor<SignUpViewModel>));
        Locator.CurrentMutable.Register(() => new WindowsUserDialogs(), typeof(IUserDialogs));

        // Show the sample page.
        Router
            .NavigateAndReset
            .Execute(new SignUpViewModel())
            .Subscribe();
    }

    /// <summary>
    /// Gets the router which is used to navigate between views.
    /// </summary>
    public RoutingState Router { get; } = new RoutingState();

    /// <summary>
    /// Creates the first main page used within the application.
    /// </summary>
    /// <returns>The page generated.</returns>
    public Window CreateMainWindow()
    {
        // NB: This returns the opening page that the platform-specific
        // boilerplate code will look for. It will know to find us because
        // we've registered our AppBootstrapScreen.
        return new MetroWindow
        {
            Width = 400,
            Height = 400,
            Content = new RoutedViewHost
            {
                Router = Router
            }
        };
    }
}
