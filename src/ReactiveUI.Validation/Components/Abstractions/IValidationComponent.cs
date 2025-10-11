// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components.Abstractions;

/// <summary>
/// Core interface which all validation components must implement.
/// </summary>
public interface IValidationComponent
{
    /// <summary>
    /// Gets the current (optional) validation message.
    /// </summary>
    IValidationText? Text { get; }

    /// <summary>
    /// Gets a value indicating whether the validation is currently valid or not.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets the observable for validation state changes.
    /// </summary>
    IObservable<IValidationState> ValidationStatusChange { get; }
}
