// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// Container for validation text.
/// </summary>
public class ValidationText : IEnumerable<string>
{
    /// <summary>
    /// The none validation text singleton instance contains no items.
    /// </summary>
    public static readonly ValidationText None = new(Array.Empty<string>());

    /// <summary>
    /// The empty validation text singleton instance contains single empty string.
    /// </summary>
    public static readonly ValidationText Empty = new(new[] { string.Empty });

    private readonly string[] _texts;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationText"/> class with the array of texts.
    /// </summary>
    /// <param name="texts">The array of texts.</param>
    private ValidationText(string[] texts) => _texts = texts;

    /// <summary>
    /// Gets returns the number of elements in the collection.
    /// </summary>
    public int Count => _texts.Length;

    /// <summary>
    /// Indexer.
    /// </summary>
    /// <param name="index">Position.</param>
    public string this[int index] => _texts[index];

    /// <summary>
    /// Combines multiple <see cref="ValidationText"/> instances into a single instance, or returns <see cref="None"/> if the
    /// enumeration is empty, or <see cref="Empty"/> if the enumeration only contains <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="validationTexts">An enumeration of <see cref="ValidationText"/>.</param>
    /// <returns>A <see cref="ValidationText"/>.</returns>
    public static ValidationText Create(IEnumerable<ValidationText>? validationTexts)
    {
        // Note _texts are already validated as not-null
        string[] texts = (validationTexts ?? Array.Empty<ValidationText>())
            .SelectMany(vt => vt._texts)
            .ToArray();

        if (texts.Length < 1)
        {
            return None;
        }

        if (texts.Length == 1 && texts[0].Length < 1)
        {
            return Empty;
        }

        return new ValidationText(texts);
    }

    /// <summary>
    /// Combines multiple validation messages into a single instance, or returns <see cref="None"/> if the enumeration is empty, or only contains empty elements.
    /// </summary>
    /// <param name="validationTexts">An enumeration of validation messages.</param>
    /// <returns>A <see cref="ValidationText"/>.</returns>
    public static ValidationText Create(IEnumerable<string>? validationTexts)
    {
        string[] texts = (validationTexts ?? Array.Empty<string>())
            .Where(t => t is not null)
            .ToArray();

        if (texts.Length < 1)
        {
            return None;
        }

        if (texts.Length == 1 && texts[0].Length < 1)
        {
            return Empty;
        }

        return new ValidationText(texts);
    }

    /// <summary>
    /// Combines multiple validation messages into a single instance, or returns <see cref="None"/> if the enumeration is empty, or contains a single empty element.
    /// </summary>
    /// <param name="validationTexts">An array of validation messages.</param>
    /// <returns>A <see cref="ValidationText"/>.</returns>
    public static ValidationText Create(params string?[]? validationTexts)
    {
        if (validationTexts is null || validationTexts.Length < 1)
        {
            return None;
        }

        // Optimise code path for single item array.
        if (validationTexts.Length == 1)
        {
            var text = validationTexts[0];

            if (text is null)
            {
                return None;
            }

            return text.Length < 1 ? Empty : new ValidationText(validationTexts!);
        }

        // Ensure we have no null items in the multi-item array
        if (validationTexts.Any(t => t is null))
        {
            // Strip nulls
            validationTexts = validationTexts.Where(t => t is not null).ToArray();
            if (validationTexts.Length < 1)
            {
                return None;
            }

            if (validationTexts[0]?.Length < 1)
            {
                return Empty;
            }
        }

        return new ValidationText(validationTexts!);
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator() => ((IEnumerable<string>)_texts).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _texts.GetEnumerator();

    /// <summary>
    /// Convert representation to a single line using a specified separator.
    /// </summary>
    /// <param name="separator">String separator.</param>
    /// <returns>Returns all the text collection separated by the separator.</returns>
    public string ToSingleLine(string? separator = ",") => string.Join(separator, _texts);
}