// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp.iOS/Application.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using UIKit;

namespace LoginApp.IOS
{
    /// <summary>
    /// Hosts the main application entry point for the application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Arguments passed to the application.</param>
        private static void Main(string[] args)
        {
            // If you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}