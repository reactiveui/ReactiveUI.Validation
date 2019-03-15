// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using LoginApp.ViewModels.Abstractions;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace LoginApp.Views
{
    public abstract class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel>
        where TViewModel : ViewModelBase
    {
        protected abstract void CreateBindings(CompositeDisposable disposables);
        
        public ContentPageBase()
        {
            On<iOS>().SetUseSafeArea(true);
            this.WhenActivated(CreateBindings);
        }
    }
}