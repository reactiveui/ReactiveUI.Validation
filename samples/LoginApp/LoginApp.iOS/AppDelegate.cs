// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp.iOS/AppDelegate.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using Foundation;
using LoginApp.Forms;
using UIKit;

namespace LoginApp.IOS
{
    /// <summary>
    /// The application delegate for the application.
    /// </summary>
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
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
