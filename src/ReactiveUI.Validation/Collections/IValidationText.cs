// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// Represents the validation state of a validation component.
/// </summary>
public interface IValidationText : IEnumerable<string>
{
    /// <summary>
    /// Gets returns the number of elements in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Indexer.
    /// </summary>
    /// <param name="index">Position.</param>
    string this[int index] { get; }

    /// <summary>
    /// Convert representation to a single line using a specified separator.
    /// </summary>
    /// <param name="separator">String separator.</param>
    /// <returns>Returns all the text collection separated by the separator.</returns>
    string ToSingleLine(string? separator = ",");
}
