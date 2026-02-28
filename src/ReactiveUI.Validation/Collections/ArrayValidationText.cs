// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// An <see cref="IValidationText"/> implementation that wraps an array of validation message strings.
/// </summary>
/// <param name="texts">The array of validation message strings.</param>
internal sealed class ArrayValidationText(string[] texts) : IValidationText
{
    /// <summary>
    /// Gets the number of validation messages in the array.
    /// </summary>
    public int Count => texts.Length;

    /// <summary>
    /// Gets the validation message at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the message to retrieve.</param>
    /// <returns>The validation message string at the specified index.</returns>
    public string this[int index] => texts[index];

    /// <summary>
    /// Returns an enumerator that iterates through the validation messages.
    /// </summary>
    /// <returns>An enumerator for the validation message strings.</returns>
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)texts).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => texts.GetEnumerator();

    /// <summary>
    /// Joins all validation messages into a single string using the specified separator.
    /// </summary>
    /// <param name="separator">The string to use as a separator between messages.</param>
    /// <returns>A single string containing all validation messages joined by the separator.</returns>
    public string ToSingleLine(string? separator) => string.Join(separator, texts);
}
