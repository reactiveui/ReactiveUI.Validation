// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using Acr.UserDialogs;
using LoginApp.ViewModels;
using LoginApp.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using Xamarin.Forms;

namespace LoginApp
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; }

        public AppBootstrapper()
        {
            Router = new RoutingState();
            RegisterViews();
            RegisterDialogs();

            this.Router
                .NavigateAndReset
                .Execute(new SignUpViewModel())
                .Subscribe();
        }

        private void RegisterViews()
        {
            Locator.CurrentMutable.RegisterConstant(this, typeof(IScreen));
            Locator.CurrentMutable.Register(() => new SignUpView(), typeof(IViewFor<SignUpViewModel>));
        }

        private void RegisterDialogs()
        {
            Locator.CurrentMutable.Register(() => UserDialogs.Instance, typeof(IUserDialogs));
        }

        public Page CreateMainPage()
        {
            // NB: This returns the opening page that the platform-specific
            // boilerplate code will look for. It will know to find us because
            // we've registered our AppBootstrapScreen.
            return new RoutedViewHost();
        }
    }
}