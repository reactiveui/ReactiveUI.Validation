// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.ValidationTexts.Abstractions;

namespace ReactiveUI.Validation.ValidationTexts;

/// <summary>
/// Factory container for validation text.
/// </summary>
public static class ValidationText
{
    /// <summary>
    /// The none validation text singleton instance contains no items.
    /// </summary>
    public static readonly IValidationText None = new ArrayValidationText(Array.Empty<string>());

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
        string[] texts = validationTexts.SelectMany(static vt => vt).ToArray();

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

        string[] texts = validationTexts.Where(t => t is not null).ToArray()!;

        return CreateValidationText(texts, texts.Length);
    }

    /// <summary>
    /// validation message into a single instance, or returns <see cref="None"/> if the enumeration is empty, or contains a single empty element.
    /// </summary>
    /// <param name="validationText">An array of validation messages.</param>
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
            string? text = validationTexts[0];

            if (text is null)
            {
                return None;
            }

            return text.Length < 1 ? Empty : new SingleValidationText(text);
        }

        string[] texts = ArrayPool<string>.Shared.Rent(validationTexts.Length);

        try
        {
            int currentIndex = 0;

            // Ensure we have no null items in the multi-item array
            for (int i = 0; i < validationTexts.Length; i++)
            {
                string? text = validationTexts[i];

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

    private static IValidationText CreateValidationText(IReadOnlyList<string> texts, int count) => count switch
    {
        0 => None,
        1 => CreateValidationText(texts[0]),
        _ => new ArrayValidationText(texts.Take(count).ToArray())
    };

    private static IValidationText CreateValidationText(string text) => text.Length is 0 ? Empty : new SingleValidationText(text);
}
