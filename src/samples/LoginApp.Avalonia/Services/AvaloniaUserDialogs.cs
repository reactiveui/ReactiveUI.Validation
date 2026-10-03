// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using LoginApp.Services;
using MsBox.Avalonia;

namespace LoginApp.Avalonia.Services;

/// <summary>This class defines user dialogs for the Avalonia app.</summary>
public class AvaloniaUserDialogs : IUserDialogs
{
    /// <inheritdoc />
    public void ShowDialog(string message) =>
        MessageBoxManager
            .GetMessageBoxStandard("Notification", message).ShowAsync();
}
