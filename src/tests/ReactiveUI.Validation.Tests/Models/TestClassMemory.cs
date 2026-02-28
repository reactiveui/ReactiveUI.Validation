// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// TestClassMemory.
/// </summary>
/// <seealso cref="ReactiveValidationObject" />
/// <seealso cref="IDisposable" />
public sealed class TestClassMemory : ReactiveValidationObject
{
    /// <summary>
    /// Composite disposable that manages validation rule and subscription lifetimes.
    /// </summary>
    private readonly CompositeDisposable _disposable = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="TestClassMemory"/> class.
    /// </summary>
    public TestClassMemory()
    {
        this.ValidationRule(
            vmp => vmp.Name,
            name => !string.IsNullOrEmpty(name),
            "The name is empty.")
            .DisposeWith(_disposable);

        // commenting out the following statement makes the test green
        ValidationContext?.ValidationStatusChange
            .Subscribe(/* you do something here, but this does not matter for now. */)
            .DisposeWith(_disposable);
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release managed resources; <c>false</c> when called from a finalizer.</param>
    protected override void Dispose(bool disposing)
    {
        if (!_disposable.IsDisposed && disposing)
        {
            _disposable.Dispose();
        }

        base.Dispose(disposing);
    }
}
