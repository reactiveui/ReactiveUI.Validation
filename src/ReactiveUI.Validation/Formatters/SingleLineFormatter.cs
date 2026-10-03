// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Formatters;
#else
namespace ReactiveUI.Validation.Formatters;
#endif

/// <inheritdoc />
/// <summary>Helper class to generate a single formatted line for a <see cref="IValidationText" />.</summary>
/// <param name="separator">Separator string.</param>
/// <remarks>
/// Initializes a new instance of the <see cref="SingleLineFormatter"/> class.
/// </remarks>
public class SingleLineFormatter(string? separator = null) : IValidationTextFormatter<string>
{
    /// <summary>Gets the default formatter.</summary>
    public static SingleLineFormatter Default { get; } = new(" ");

    /// <summary>Formats the <see cref="IValidationText"/> into a single line text using the default separator.</summary>
    /// <param name="validationText">ValidationText object to be formatted.</param>
    /// <returns>Returns the string formatted.</returns>
    public string Format(IValidationText? validationText) =>
        validationText is not null
            ? validationText.ToSingleLine(separator)
            : string.Empty;
}
