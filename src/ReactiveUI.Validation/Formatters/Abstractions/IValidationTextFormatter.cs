using ReactiveUI.Validation.Collections;

namespace ReactiveUI.Validation.Formatters.Abstractions
{
    /// <summary>
    ///     Specification for a <see cref="ValidationText" /> formatter.
    /// </summary>
    /// <typeparam name="TOut"></typeparam>
    public interface IValidationTextFormatter<out TOut>
    {
        TOut Format(ValidationText validationText);
    }
}