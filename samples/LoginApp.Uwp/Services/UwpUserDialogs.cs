// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using LoginApp.Services;
using Windows.UI.Popups;

namespace LoginApp.Uwp.Services
{
    /// <summary>
    /// This class defines user dialogs for the Universal Windows Platform app.
    /// </summary>
    public class UwpUserDialogs : IUserDialogs
    {
        /// <inheritdoc />
        public void ShowDialog(string message) => new MessageDialog(message).ShowAsync();
    }
}
