// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
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
    public string Name { get; set; }

    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected override void Dispose(bool disposing)
    {
        if (!_disposable.IsDisposed && disposing)
        {
            _disposable.Dispose();
        }

        base.Dispose(disposing);
    }
}
