// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// An <see cref="IValidationText"/> implementation that wraps a single validation message string.
/// </summary>
/// <param name="text">The single validation message string.</param>
internal sealed class SingleValidationText(string text) : IValidationText
{
    /// <summary>
    /// Gets the number of validation messages, which is always 1.
    /// </summary>
    public int Count => 1;

    /// <summary>
    /// Gets the validation message at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the message to retrieve.</param>
    /// <returns>The validation message string.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is not 0.</exception>
    public string this[int index] => index is 0 ? text : throw new ArgumentOutOfRangeException(nameof(index));

    /// <summary>
    /// Returns an enumerator that yields the single validation message.
    /// </summary>
    /// <returns>An enumerator that yields the single validation message.</returns>
    public IEnumerator<string> GetEnumerator()
    {
        yield return text;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Returns the single validation message as a single-line string.
    /// </summary>
    /// <param name="separator">Ignored because there is only one message.</param>
    /// <returns>The validation message string.</returns>
    public string ToSingleLine(string? separator) => text;
}
