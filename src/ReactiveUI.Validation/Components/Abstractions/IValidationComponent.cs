using System;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components.Abstractions
{
    /// <summary>
    /// Core interface which all validation components must implement.
    /// </summary>
    public interface IValidationComponent
    {
        /// <summary>
        /// Gets the current (optional) validation message.
        /// </summary>
        ValidationText Text { get; }

        /// <summary>
        /// Gets the current validation state.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the observable for validation state changes.
        /// </summary>
        IObservable<ValidationState> ValidationStatusChange { get; }
    }
}