// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace LoginApp.Forms.IOS
{
    /// <summary>
    /// The application delegate for the application.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        /// <inheritdoc/>
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}
