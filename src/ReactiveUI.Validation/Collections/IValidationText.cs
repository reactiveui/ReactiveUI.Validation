// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// Represents the validation state of a validation component.
/// </summary>
public interface IValidationText : IReadOnlyList<string>
{
    /// <summary>
    /// Convert representation to a single line using a specified separator.
    /// </summary>
    /// <param name="separator">String separator.</param>
    /// <returns>Returns all the text collection separated by the separator.</returns>
    string ToSingleLine(string? separator = ",");
}
