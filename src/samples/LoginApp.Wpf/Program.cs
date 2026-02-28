// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace LoginApp.Wpf;

/// <summary>
/// Windows Presentation Framework entry point.
/// </summary>
public static class Program
{
    /// <summary>
    /// The entry point of the .NET Core GUI application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    [STAThread]
    public static void Main(string[] args) => new App().Run();
}
