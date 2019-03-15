using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.Formatters.Abstractions
{
    /// <summary>
    /// Specification for a <see cref="ValidationText"/> formatter.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IValidationTextFormatter<out TOut>
    {
        /// <summary>
        /// Formats the <see cref="ValidationText"/> to desired output.
        /// </summary>
        /// <param name="validationText"></param>
        /// <returns></returns>
        TOut Format(ValidationText validationText);
    }
}