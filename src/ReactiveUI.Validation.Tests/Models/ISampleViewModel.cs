// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Abstractions;

namespace ReactiveUI.Validation.Tests.Models;

/// <summary>
/// Sample abstract view model that implements <see cref="IReactiveObject" />.
/// </summary>
public interface ISampleViewModel : IReactiveObject, IValidatableViewModel
{
    /// <summary>
    /// Gets or sets the name property used for testing.
    /// </summary>
    string Name { get; set; }
}