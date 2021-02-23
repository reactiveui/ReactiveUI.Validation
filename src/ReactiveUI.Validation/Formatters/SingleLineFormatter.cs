// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Formatters.Abstractions;

namespace ReactiveUI.Validation.Formatters
{
    /// <inheritdoc />
    /// <summary>
    /// Helper class to generate a single formatted line for a <see cref="ReactiveUI.Validation.Collections.ValidationText" />.
    /// </summary>
    public class SingleLineFormatter : IValidationTextFormatter<string>
    {
        private readonly string? _separator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleLineFormatter"/> class.
        /// </summary>
        /// <param name="separator">Separator string.</param>
        public SingleLineFormatter(string? separator = null) => _separator = separator;

        /// <summary>
        /// Gets the default formatter.
        /// </summary>
        public static SingleLineFormatter Default { get; } = new(" ");

        /// <summary>
        /// Formats the <see cref="ValidationText"/> into a single line text using the
        /// default separator.
        /// </summary>
        /// <param name="validationText">ValidationText object to be formatted.</param>
        /// <returns>Returns the string formatted.</returns>
        public string Format(ValidationText? validationText) =>
            validationText is not null
                ? validationText.ToSingleLine(_separator)
                : string.Empty;
    }
}
