// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reactive.Disposables;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace LoginApp.Views
{
    public abstract class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : ViewModelBase
    {
        protected abstract void CreateBindings(CompositeDisposable disposables);
        
        public ContentPageBase()
        {
            this.WhenActivated(CreateBindings);
        }
    }
}