// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using LoginApp.Services;

namespace LoginApp.Wpf.Services;

/// <summary>
/// This class defines user dialogs for the Windows Presentation Framework app.
/// </summary>
public class WindowsUserDialogs : IUserDialogs
{
    /// <inheritdoc />
    public void ShowDialog(string message) => MessageBox.Show(message);
}
