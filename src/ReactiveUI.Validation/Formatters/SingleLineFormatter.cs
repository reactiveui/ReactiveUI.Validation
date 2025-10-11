// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Formatters.Abstractions;

namespace ReactiveUI.Validation.Formatters;

/// <inheritdoc />
/// <summary>
/// Helper class to generate a single formatted line for a <see cref="IValidationText" />.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SingleLineFormatter"/> class.
/// </remarks>
/// <param name="separator">Separator string.</param>
public class SingleLineFormatter(string? separator = null) : IValidationTextFormatter<string>
{
    /// <summary>
    /// Gets the default formatter.
    /// </summary>
    public static SingleLineFormatter Default { get; } = new(" ");

    /// <summary>
    /// Formats the <see cref="IValidationText"/> into a single line text using the
    /// default separator.
    /// </summary>
    /// <param name="validationText">ValidationText object to be formatted.</param>
    /// <returns>Returns the string formatted.</returns>
    public string Format(IValidationText? validationText) =>
        validationText is not null
            ? validationText.ToSingleLine(separator)
            : string.Empty;
}
