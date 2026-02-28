// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI.Validation.Collections;

/// <summary>
/// Factory container for validation text.
/// </summary>
public static class ValidationText
{
    /// <summary>
    /// The none validation text singleton instance contains no items.
    /// </summary>
    public static readonly IValidationText None = new ArrayValidationText([]);

    /// <summary>
    /// The empty validation text singleton instance contains single empty string.
    /// </summary>
    public static readonly IValidationText Empty = new SingleValidationText(string.Empty);

    /// <summary>
    /// Combines multiple <see cref="IValidationText"/> instances into a single instance, or returns <see cref="None"/> if the
    /// enumeration is empty, or <see cref="Empty"/> if the enumeration only contains <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="validationTexts">An enumeration of <see cref="IValidationText"/>.</param>
    /// <returns>A <see cref="IValidationText"/>.</returns>
    public static IValidationText Create(IEnumerable<IValidationText>? validationTexts)
    {
        if (validationTexts is null)
        {
            return None;
        }

        // Note _texts are already validated as not-null
        var texts = validationTexts.SelectMany(static vt => vt).ToArray();

        return CreateValidationText(texts, texts.Length);
    }

    /// <summary>
    /// Combines multiple validation messages into a single instance, or returns <see cref="None"/> if the enumeration is empty, or only contains empty elements.
    /// </summary>
    /// <param name="validationTexts">An enumeration of validation messages.</param>
    /// <returns>A <see cref="IValidationText"/>.</returns>
    public static IValidationText Create(IEnumerable<string?>? validationTexts)
    {
        if (validationTexts is null)
        {
            return None;
        }

        var texts = validationTexts.Where(t => t is not null).ToArray();

        return CreateValidationText(texts!, texts.Length);
    }

    /// <summary>
    /// Wraps a single validation message into an <see cref="IValidationText"/> instance, or returns <see cref="None"/> if the message is null.
    /// </summary>
    /// <param name="validationText">A single validation message.</param>
    /// <returns>A <see cref="IValidationText"/>.</returns>
    public static IValidationText Create(string? validationText) => validationText is null ? None : CreateValidationText(validationText);

    /// <summary>
    /// Combines multiple validation messages into a single instance, or returns <see cref="None"/> if the enumeration is empty, or contains a single empty element.
    /// </summary>
    /// <param name="validationTexts">An array of validation messages.</param>
    /// <returns>A <see cref="IValidationText"/>.</returns>
    public static IValidationText Create(params string?[]? validationTexts)
    {
        if (validationTexts is null || validationTexts.Length < 1)
        {
            return None;
        }

        // Optimize code path for single item array.
        if (validationTexts.Length == 1)
        {
            var text = validationTexts[0];

            if (text is null)
            {
                return None;
            }

            return text.Length < 1 ? Empty : new SingleValidationText(text);
        }

        var texts = ArrayPool<string>.Shared.Rent(validationTexts.Length);

        try
        {
            var currentIndex = 0;

            // Ensure we have no null items in the multi-item array
            for (var i = 0; i < validationTexts.Length; i++)
            {
                var text = validationTexts[i];

                if (text is null)
                {
                    continue;
                }

                texts[currentIndex] = text;
                currentIndex++;
            }

            return CreateValidationText(texts, currentIndex);
        }
        finally
        {
            ArrayPool<string>.Shared.Return(texts);
        }
    }

    /// <summary>
    /// Selects the appropriate <see cref="IValidationText"/> implementation based on the number of messages.
    /// Returns <see cref="None"/> for zero, a <see cref="SingleValidationText"/> for one, or an
    /// <see cref="ArrayValidationText"/> for multiple messages.
    /// </summary>
    /// <param name="texts">The source list of validation message strings.</param>
    /// <param name="count">The number of valid messages to use from the list.</param>
    /// <returns>An <see cref="IValidationText"/> appropriate for the message count.</returns>
    internal static IValidationText CreateValidationText(IReadOnlyList<string> texts, int count) => count switch
    {
        0 => None,
        1 => CreateValidationText(texts[0]),
        _ when texts is string[] array && count == array.Length => new ArrayValidationText(array),
        _ when texts is string[] array => new ArrayValidationText(CopyArray(array, count)),
        _ => new ArrayValidationText([.. texts.Take(count)])
    };

    /// <summary>
    /// Creates an <see cref="IValidationText"/> for a single message string, returning <see cref="Empty"/>
    /// for zero-length strings or a new <see cref="SingleValidationText"/> otherwise.
    /// </summary>
    /// <param name="text">The validation message string.</param>
    /// <returns>An <see cref="IValidationText"/> wrapping the message.</returns>
    internal static IValidationText CreateValidationText(string text) => text.Length is 0 ? Empty : new SingleValidationText(text);

    /// <summary>
    /// Copies the first <paramref name="count"/> elements from the source array into a new array.
    /// </summary>
    /// <param name="source">The source array to copy from.</param>
    /// <param name="count">The number of elements to copy.</param>
    /// <returns>A new array containing the first <paramref name="count"/> elements.</returns>
    internal static string[] CopyArray(string[] source, int count)
    {
        var result = new string[count];
        Array.Copy(source, result, count);
        return result;
    }
}
