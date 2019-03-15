using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// Encapsulation of a validation with bindable properties.
    /// </summary>
    public class ValidationHelper : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IValidationComponent _validation;

        private ObservableAsPropertyHelper<bool> _isValid;
        private ObservableAsPropertyHelper<ValidationText> _message;

        /// <inheritdoc />
        public ValidationHelper(IValidationComponent validation)
        {
            _validation = validation;
            Setup();
        }

        /// <summary>
        /// Gets the current validation state.
        /// </summary>
        public bool IsValid => _isValid.Value;

        /// <summary>
        /// Gets the current (optional) validation message.
        /// </summary>
        public ValidationText Message => _message.Value;

        /// <summary>
        /// Gets the observable for validation state changes.
        /// </summary>
        public IObservable<ValidationState> ValidationChanged => _validation.ValidationStatusChange;

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables?.Dispose();
        }

        private void Setup()
        {
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.IsValid)
                .ToProperty(this, vm => vm.IsValid, out _isValid));
            _disposables.Add(_validation.ValidationStatusChange.Select(v => v.Text)
                .ToProperty(this, vm => vm.Message, out _message));
        }
    }
}