// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections
{
    /// <summary>
    /// Container for validation text.
    /// </summary>
    public class ValidationText : IEnumerable<string>
    {
        private readonly List<string> _texts = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        public ValidationText()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="text">Text to be added in the collection.</param>
        public ValidationText(string text)
        {
            _texts.Add(text);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationText"/> class.
        /// </summary>
        /// <param name="validationTexts"><see cref="ValidationText"/> collection to be added into the text collection.</param>
        public ValidationText(IEnumerable<ValidationText> validationTexts)
        {
            if (validationTexts is null)
            {
                throw new System.ArgumentNullException(nameof(validationTexts));
            }

            foreach (var text in validationTexts)
            {
                _texts.AddRange(text._texts);
            }
        }

        /// <summary>
        /// Gets returns the number of elements in the collection.
        /// </summary>
        public int Count => _texts.Count;

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index">Position.</param>
        public string this[int index] => _texts[index];

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator()
        {
            return _texts.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds a text to the collection.
        /// </summary>
        /// <param name="text">Text to be added in the collection.</param>
        public void Add(string text)
        {
            _texts.Add(text);
        }

        /// <summary>
        /// Clear all texts.
        /// </summary>
        public void Clear()
        {
            _texts.Clear();
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
