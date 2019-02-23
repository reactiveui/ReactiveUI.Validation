using ReactiveUI.Validation.Contexts;

namespace ReactiveUI.Validation.Abstractions
{
    /// <summary>
    ///     Interface used by view models to indicate they have a validation context.
    /// </summary>
    public interface ISupportsValidation
    {
        /// <summary>
        ///     Get the validation context
        /// </summary>
        ValidationContext ValidationContext { get; }
    }
}