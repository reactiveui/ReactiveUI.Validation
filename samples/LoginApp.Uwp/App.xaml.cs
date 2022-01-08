// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using LoginApp.Uwp.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LoginApp.Uwp
{
    /// <summary>
    /// Defines the main Universal Windows Application class.
    /// </summary>
    /// <inheritdoc />
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App() => InitializeComponent();

        /// <inheritdoc />
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                Window.Current.Content = rootFrame;
            }

            if (e?.PrelaunchActivated == true)
            {
                return;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(SignUpView), e?.Arguments);
            }

            Window.Current.Activate();
        }
    }
}
