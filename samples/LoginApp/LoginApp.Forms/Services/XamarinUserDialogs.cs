// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using LoginApp.Services;
using Xamarin.Forms;

namespace LoginApp.Forms.Services
{
    /// <summary>
    /// This class defines user dialogs for the Xamarin Forms app.
    /// </summary>
    public class XamarinUserDialogs : IUserDialogs
    {
        private readonly Application _application;

        /// <summary>
        /// Initializes a new instance of the <see cref="XamarinUserDialogs"/> class.
        /// </summary>
        /// <param name="application">Xamarin Forms application instance.</param>
        public XamarinUserDialogs(Application application) => _application = application;

        /// <inheritdoc />
        public void ShowDialog(string message) => _application.MainPage.DisplayAlert("Notification", message, "Ok");
    }
}
