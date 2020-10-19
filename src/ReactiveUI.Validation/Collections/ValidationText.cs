// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ReactiveUI.Validation.Collections
{
    /// <summary>
    /// Container for validation text.
    /// </summary>
    public class ValidationText : IEnumerable<string>
    {
        /// <summary>
        /// The empty validation text singleton instance.
        /// </summary>
        public static readonly ValidationText Empty = new ValidationText(Array.Empty<string>());

        // TODO When Add() & Clear() are obsolesced this should be made read-only.
        private /* readonly */ string[] _texts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create() overload instead.")]
        public ValidationText()
            : this(Array.Empty<string>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="text">Text to be added in the collection.</param>
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create(string) overload instead.")]
        public ValidationText(string text)
        :this(new[] { text })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="validationTexts"><see cref="ValidationText"/> collection to be added into the text collection.</param>
        [Obsolete("Calling the constructor is deprecated, please use ValidationText.Create(IEnumerable<ValidationText>) overload instead.")]
        public ValidationText(IEnumerable<ValidationText> validationTexts)
            : this((validationTexts ?? throw new System.ArgumentNullException(nameof(validationTexts))).SelectMany(vt => vt._texts).ToArray())
        {
        }

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
        /// Combines multiple <see cref="ValidationText"/> instances into a single instance, or returns <see cref="Empty"/> if the enumeration is empty.
        /// </summary>
        /// <param name="validationTexts">An enumeration of <see cref="ValidationText"/>.</param>
        /// <returns>A <see cref="ValidationText"/>.</returns>
        public static ValidationText Create(IEnumerable<ValidationText> validationTexts) =>
            Create(validationTexts.SelectMany(vt => vt._texts).ToArray());

        /// <summary>
        /// Combines multiple validation messages into a single instance, or returns <see cref="Empty"/> if the enumeration is empty.
        /// </summary>
        /// <param name="validationTexts">An enumeration of validation messages.</param>
        /// <returns>A <see cref="ValidationText"/>.</returns>
        public static ValidationText Create(IEnumerable<string> validationTexts) => Create(validationTexts.ToArray());

        /// <summary>
        /// Combines multiple validation messages into a single instance, or returns <see cref="Empty"/> if the enumeration is empty.
        /// </summary>
        /// <param name="validationTexts">An array of validation messages.</param>
        /// <returns>A <see cref="ValidationText"/>.</returns>
        public static ValidationText Create(params string[] validationTexts) => validationTexts.Length > 0
            ? new ValidationText(validationTexts)
            : Empty;

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
        [Obsolete("ValidationText will be made immutable in future versions, please do not use the Add(string) method.")]
        public void Add(string text)
        {
            _texts = _texts.Concat(new[] { text }).ToArray();
        }

        /// <summary>
        /// Clear all texts.
        /// </summary>
        [Obsolete("ValidationText will be made immutable in future versions, please do not use the Clear() method.")]
        public void Clear()
        {
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
