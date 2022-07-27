// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.States;
using ReactiveUI.Validation.ValidationTexts.Abstractions;

namespace ReactiveUI.Validation.Components.Abstractions;

/// <summary>
/// Core interface which all validation components must implement.
/// </summary>
public interface IValidationComponent
{
    /// <summary>
    /// Gets the current (optional) validation message.
    /// </summary>
    public IValidationText? Text { get; }

    /// <summary>
    /// Gets a value indicating whether the validation is currently valid or not.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the observable for validation state changes.
    /// </summary>
    public IObservable<IValidationState> ValidationStatusChange { get; }
}
