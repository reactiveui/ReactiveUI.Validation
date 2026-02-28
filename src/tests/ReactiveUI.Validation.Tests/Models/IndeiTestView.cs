// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Mocked View for INotifyDataErrorInfo testing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IndeiTestView"/> class.
/// </remarks>
/// <param name="viewModel">ViewModel instance of type <see cref="TestViewModel"/>.</param>
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

    /// <summary>
    /// Gets or sets the Name Label which emulates a Text property (eg. Entry in Xamarin.Forms).
    /// </summary>
    public string NameLabel { get; set; } = null!;

    /// <summary>
    /// Gets or sets the NameError Label which emulates a Text property (eg. Entry in Xamarin.Forms).
    /// </summary>
    public string NameErrorLabel { get; set; } = null!;
}
