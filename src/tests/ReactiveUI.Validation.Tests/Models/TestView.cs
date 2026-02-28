// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Represents a mocked view used by the tests under <c>ReactiveUI.Validation.Tests.Models</c>.
/// Implements <see cref="IViewFor{T}"/> for <see cref="TestViewModel"/> and exposes
/// simple properties that emulate UI elements used in validation scenarios.
/// </summary>
public class TestView : ReactiveObject, IViewFor<TestViewModel>
{

    /// <summary>
    /// Initializes a new instance of the <see cref="TestView"/> class.
    /// </summary>
    public TestView()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestView"/> class with the specified view model.
    /// </summary>
    /// <param name="viewModel">The <see cref="TestViewModel"/> instance to associate with this view.</param>
    public TestView(TestViewModel viewModel) => ViewModel = viewModel;

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TestViewModel;
    }

    /// <summary>
    /// Gets or sets the view model associated with this view.
    /// </summary>
    public TestViewModel? ViewModel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>
    /// Gets or sets the Name label text which emulates a <c>Text</c> property (e.g., Entry in Xamarin.Forms).
    /// </summary>
    public string NameLabel { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Name2 label text which emulates a <c>Text</c> property (e.g., Entry in Xamarin.Forms).
    /// </summary>
    public string Name2Label { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Name error label text which emulates a <c>Text</c> property (e.g., Entry in Xamarin.Forms).
    /// </summary>
    public string NameErrorLabel { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Name2 error label text which emulates a <c>Text</c> property (e.g., Entry in Xamarin.Forms).
    /// </summary>
    public string Name2ErrorLabel { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether the Name field is valid.
    /// </summary>
    public bool IsNameValid { get; set; }

    /// <summary>
    /// Gets the container control for the Name validation error message.
    /// </summary>
    public TestControl NameErrorContainer { get; } = new();
}
