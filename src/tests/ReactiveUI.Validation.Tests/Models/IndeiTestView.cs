// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Mocked View for INotifyDataErrorInfo testing.</summary>
/// <param name="viewModel">ViewModel instance of type <see cref="TestViewModel"/>.</param>
/// <remarks>
/// Initializes a new instance of the <see cref="IndeiTestView"/> class.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("IndeiTestView: {ViewModel}")]
public class IndeiTestView(IndeiTestViewModel viewModel) : IViewFor<IndeiTestViewModel>
{
    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as IndeiTestViewModel;
    }

    /// <inheritdoc/>
    public IndeiTestViewModel? ViewModel { get; set; } = viewModel;

    /// <summary>Gets or sets the Name Label which emulates a Text property (eg. Entry in Xamarin.Forms).</summary>
    public string NameLabel { get; set; } = null!;

    /// <summary>Gets or sets the NameError Label which emulates a Text property (eg. Entry in Xamarin.Forms).</summary>
    public string NameErrorLabel { get; set; } = null!;
}
