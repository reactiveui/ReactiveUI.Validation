// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/AppBootstrapper.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using LoginApp.Forms.Services;
using LoginApp.Forms.Views;
using LoginApp.Services;
using LoginApp.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using Splat;

namespace LoginApp.Forms
{
    /// <summary>
    /// The app bootstrapper which is used to register everything with the Splat service locator.
    /// It is also the central location for the RoutingState used for routing between views.
    /// </summary>
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppBootstrapper"/> class.
        /// </summary>
        public AppBootstrapper(Application application)
        {
            // Register the dependencies.
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new SignUpView(), typeof(IViewFor<SignUpViewModel>));
            Locator.CurrentMutable.Register(() => new XamarinUserDialogs(application), typeof(IUserDialogs));
            
            // Show the sample page.
            Router
                .NavigateAndReset
                .Execute(new SignUpViewModel())
                .Subscribe();
        }

        /// <summary>
        /// Gets or sets the router which is used to navigate between views.
        /// </summary>
        public RoutingState Router { get; } = new RoutingState();

        /// <summary>
        /// Creates the first main page used within the application.
        /// </summary>
        /// <returns>The page generated.</returns>
        public Page CreateMainPage()
        {
            // NB: This returns the opening page that the platform-specific
            // boilerplate code will look for. It will know to find us because
            // we've registered our AppBootstrapScreen.
            return new RoutedViewHost();
        }
    }
}
