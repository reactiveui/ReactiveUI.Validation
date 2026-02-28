// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Abstractions;

/// <summary>
/// Interface used by view models to indicate they have a validation context.
/// </summary>
public interface IValidatableViewModel
{
    /// <summary>
    /// Gets the validation context.
    /// </summary>
    IValidationContext ValidationContext { get; }
}
