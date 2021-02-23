// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using LoginApp.Services;

namespace LoginApp.Avalonia.Services
{
    /// <summary>
    /// This class defines user dialogs for the Avalonia app.
    /// </summary>
    public class AvaloniaUserDialogs : IUserDialogs
    {
        /// <inheritdoc />
        public void ShowDialog(string message) =>
            MessageBox
                .Avalonia
                .MessageBoxManager
                .GetMessageBoxStandardWindow("Notification", message).Show();
    }
}
