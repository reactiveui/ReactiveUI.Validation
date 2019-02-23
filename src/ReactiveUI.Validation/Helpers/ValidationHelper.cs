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
    ///     Encapsulation of a validation with bindable properties.
    /// </summary>
    public class ValidationHelper : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IValidationComponent _validation;

        // how do we get this to be reactive though? we need to publish 
        // validation object
        private ObservableAsPropertyHelper<bool> _isValid;

        private ObservableAsPropertyHelper<ValidationText> _message;

        public ValidationHelper(IValidationComponent validation)
        {
            _validation = validation;
            Setup();
        }

        public bool IsValid => _isValid.Value;
        public ValidationText Message => _message.Value;

        public IObservable<ValidationState> ValidationChanged => _validation.ValidationStatusChange;

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