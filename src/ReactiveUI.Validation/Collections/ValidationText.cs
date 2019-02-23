using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections
{
    /// <summary>
    ///     Container for validation text
    /// </summary>
    public class ValidationText : IEnumerable<string>
    {
        private readonly List<string> _texts = new List<string>();

        public ValidationText()
        {
        }

        public ValidationText(string text)
        {
            _texts.Add(text);
        }

        public ValidationText(IEnumerable<ValidationText> validationTexts)
        {
            foreach (var vt in validationTexts) _texts.AddRange(vt._texts);
        }

        public string this[int index] => _texts[index];

        public int Count => _texts.Count;

        public IEnumerator<string> GetEnumerator()
        {
            return _texts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string text)
        {
            _texts.Add(text);
        }

        public void Clear()
        {
            _texts.Clear();
        }

        /// <summary>
        ///     Convert representation to a single line using a specified separator
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string ToSingleLine(string separator = ",")
        {
            return string.Join(separator, _texts);
        }
    }
}