// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ReactiveUI.Validation.Collections
{
    /// <summary>
    /// Container for validation text.
    /// </summary>
    public class ValidationText : IEnumerable<string>
    {
        /// <summary>
        /// The none validation text singleton instance contains no items.
        /// </summary>
        public static readonly ValidationText None = new ValidationText(Array.Empty<string>());

        /// <summary>
        /// The empty validation text singleton instance contains single empty string.
        /// </summary>
        public static readonly ValidationText Empty = new ValidationText(new[] { string.Empty });

        private /* readonly */ string[] _texts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create() overload instead.")]
        public ValidationText()
        {
            _texts = Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="text">Text to be added in the collection.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create(string) overload instead.")]
        public ValidationText(string text)
        {
            _texts = text is null
                ? None._texts
                : text.Length < 1
                    ? Empty._texts
                    : new[] { text };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="validationTexts"><see cref="ValidationText"/> collection to be added into the text collection.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create(IEnumerable<ValidationText>) overload instead.")]
        public ValidationText(IEnumerable<ValidationText> validationTexts)
        {
            // Note _texts are already validated as not-null
            _texts = (validationTexts ?? Array.Empty<ValidationText>())
                .SelectMany(vt => vt._texts)
                .ToArray();

            // Re-use arrays when possible
            if (_texts.Length < 1)
            {
                _texts = Array.Empty<string>();
            }
            else if (_texts.Length == 1 && _texts[0].Length < 1)
            {
                _texts = Empty._texts;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class with the array of texts.
        /// </summary>
        /// <param name="texts">The array of texts.</param>
        private ValidationText(string[] texts)
        {
            _texts = texts;
        }

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
        public static ValidationText Create(IEnumerable<ValidationText> validationTexts)
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
        public static ValidationText Create(IEnumerable<string> validationTexts)
        {
            string[] texts = (validationTexts ?? Array.Empty<string>())
                .Where(t => t != null)
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
        public static ValidationText Create(params string[] validationTexts)
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

                if (text.Length < 1)
                {
                    return Empty;
                }

                return new ValidationText(validationTexts);
            }

            // Ensure we have no null items in the multi-item array
            if (validationTexts.Any(t => t is null))
            {
                // Strip nulls
                validationTexts = validationTexts.Where(t => t != null).ToArray();
                if (validationTexts.Length < 1)
                {
                    return None;
                }

                if (validationTexts[0].Length < 1)
                {
                    return Empty;
                }
            }

            return new ValidationText(validationTexts);
        }

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            return ((IEnumerable<string>)_texts).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _texts.GetEnumerator();
        }

        /// <summary>
        /// Adds a text to the collection.
        /// </summary>
        /// <param name="text">Text to be added in the collection.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("ValidationText will be made immutable in future versions, please do not use the Add(string) method.")]
        public void Add(string text)
        {
            if (ReferenceEquals(this, Empty))
            {
                throw new InvalidOperationException("Adding to ValidationText.Empty is unsupported.");
            }

            if (ReferenceEquals(this, None))
            {
                throw new InvalidOperationException("Adding to ValidationText.None is unsupported.");
            }

            _texts = _texts.Concat(new[] { text }).ToArray();
        }

        /// <summary>
        /// Clear all texts.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("ValidationText will be made immutable in future versions, please do not use the Clear() method.")]
        public void Clear()
        {
            if (ReferenceEquals(this, Empty))
            {
                throw new InvalidOperationException("Adding to ValidationText.Empty is unsupported.");
            }

            if (ReferenceEquals(this, None))
            {
                throw new InvalidOperationException("Clearing ValidationText.None is unsupported.");
            }

            _texts = Array.Empty<string>();
        }

        /// <summary>
        /// Convert representation to a single line using a specified separator.
        /// </summary>
        /// <param name="separator">String separator.</param>
        /// <returns>Returns all the text collection separated by the separator.</returns>
        public string ToSingleLine(string? separator = ",")
        {
            return string.Join(separator, _texts);
        }
    }
}
