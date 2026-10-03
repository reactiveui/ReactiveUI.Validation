// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Mocked View.</summary>
/// <param name="viewModel">ViewModel instance of type <see cref="SourceDestinationViewModel"/>.</param>
/// <remarks>
/// Initializes a new instance of the <see cref="SourceDestinationView"/> class.
/// </remarks>
[System.Diagnostics.DebuggerDisplay("SourceDestinationView: {ViewModel}")]
public class SourceDestinationView(SourceDestinationViewModel viewModel) : IViewFor<SourceDestinationViewModel>
{
    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as SourceDestinationViewModel;
    }

    /// <inheritdoc/>
    public SourceDestinationViewModel? ViewModel { get; set; } = viewModel;

    /// <summary>Gets or sets the SourceError Label which emulates a Text property (eg. Entry in Xamarin.Forms).</summary>
    public string SourceError { get; set; } = null!;

    /// <summary>Gets or sets the DestinationError Label which emulates a Text property (eg. Entry in Xamarin.Forms).</summary>
    public string DestinationError { get; set; } = null!;
}
