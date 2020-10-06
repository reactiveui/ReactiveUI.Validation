// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/ViewModels/Abstractions/ViewModelBase.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using ReactiveUI;
using Splat;

namespace LoginApp.ViewModels
{
    /// <summary>
    /// A base for all the different view models used throughout the application.
    /// </summary>
    public abstract class ViewModelBase : ReactiveObject, IRoutableViewModel, ISupportsActivation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        /// <param name="title">The title of the view model for routing purposes.</param>
        /// <param name="hostScreen">The screen used for routing purposes.</param>
        protected ViewModelBase(string title, IScreen hostScreen = null)
        {
            UrlPathSegment = title;
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
        }

        /// <summary>
        /// Gets the current page path.
        /// </summary>
        public string UrlPathSegment { get; }

        /// <summary>
        /// Gets or sets the screen used for routing operations.
        /// </summary>
        public IScreen HostScreen { get; protected set; }

        /// <summary>
        /// Gets the activator which contains context information for use in activation of the view model.
        /// </summary>
        public ViewModelActivator Activator { get; } = new ViewModelActivator();
    }
}