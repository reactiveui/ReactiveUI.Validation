using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections
{
    /// <summary>
    /// Container for validation text
    /// </summary>
    public class ValidationText : IEnumerable<string>
    {
        private readonly List<string> _texts = new List<string>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ValidationText()
        {
        }

        /// <summary>
        /// Constructor which adds the text into the collection.
        /// </summary>
        /// <param name="text"></param>
        public ValidationText(string text)
        {
            _texts.Add(text);
        }

        /// <summary>
        /// Constructor which adds a <see cref="ValidationText"/> collection into the text collection.
        /// </summary>
        /// <param name="validationTexts"></param>
        public ValidationText(IEnumerable<ValidationText> validationTexts)
        {
            foreach (var text in validationTexts)
            {
                _texts.AddRange(text._texts);
            }
        }

        /// <summary>
        /// Indexer.
        /// </summary>
        /// <param name="index"></param>
        public string this[int index] => _texts[index];

        /// <summary>
        /// Returns the number of elements in the collection.
        /// </summary>
        public int Count => _texts.Count;

        /// <summary>
        /// Get the collection enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> GetEnumerator()
        {
            return _texts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Add a <see cref="text"/> to the collection.
        /// </summary>
        /// <param name="text"></param>
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
        /// Convert representation to a single line using a specified separator
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToSingleLine(string separator = ",")
        {
            return string.Join(separator, _texts);
        }
    }
}