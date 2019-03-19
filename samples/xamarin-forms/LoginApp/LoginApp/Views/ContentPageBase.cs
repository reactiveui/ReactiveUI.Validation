// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/Views/ContentPageBase.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System.Reactive.Disposables;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace LoginApp.Views
{
    /// <inheritdoc />
    /// <summary>
    /// A base page used for all our content pages. It is mainly used for interaction registrations.
    /// </summary>
    /// <typeparam name="TViewModel">The view model which the page contains.</typeparam>
    public abstract class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPageBase{TViewModel}"/> class.
        /// </summary>
        protected ContentPageBase()
        {
            On<iOS>().SetUseSafeArea(true);
            this.WhenActivated(CreateBindings);
        }

        /// <summary>
        /// Creates the necessary bindings for the view wrapped in the WhenActivated block.
        /// </summary>
        /// <param name="disposables">Disposable resources.</param>
        protected abstract void CreateBindings(CompositeDisposable disposables);
    }
}