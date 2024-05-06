// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.States;

/// <summary>
/// Represents the validation state of a validation component.
/// </summary>
public interface IValidationState
{
    /// <summary>
    /// Gets the validation text.
    /// </summary>
    IValidationText Text { get; }

    /// <summary>
    /// Gets a value indicating whether the validation is currently valid or not.
    /// </summary>
    bool IsValid { get; }
}
