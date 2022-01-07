// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Abstractions;

/// <summary>
/// Interface used by view models to indicate they have a validation context.
/// </summary>
public interface IValidatableViewModel
{
    /// <summary>
    /// Gets get the validation context.
    /// </summary>
    ValidationContext ValidationContext { get; }
}