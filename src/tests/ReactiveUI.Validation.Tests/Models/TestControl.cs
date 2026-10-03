// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>The container that contains a Text property.</summary>
[System.Diagnostics.DebuggerDisplay("TestControl: {Text}")]
public class TestControl
{
    /// <summary>Gets or sets the text property of the container.</summary>
    public string Text { get; set; } = null!;
}
