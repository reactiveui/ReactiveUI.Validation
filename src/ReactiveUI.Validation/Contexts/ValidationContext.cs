using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Legacy;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Contexts
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <summary>
    ///     The overall context for a view model under which validation takes place.
    /// </summary>
    /// <remarks>
    ///     Contains all of the <see cref="T:ReactiveUI.Validation.Components.Contracts.IValidationComponent" /> instances
    ///     applicable to the view model.
    /// </remarks>
    public class ValidationContext : ReactiveObject, IDisposable, IValidationComponent
    {
        /// <summary>
        ///     What needs to be disposed off
        /// </summary>
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        ///     Backing field for the current validation state
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        private readonly IConnectableObservable<bool> _validationConnectable;

        /// <summary>
        ///     The list of current validations
        /// </summary>
        private readonly ReactiveList<IValidationComponent> _validations = new ReactiveList<IValidationComponent>();

        private readonly ReplaySubject<ValidationState> _validationStatusChange = new ReplaySubject<ValidationState>(1);

        /// <summary>
        ///     Backing field for the validation summary
        /// </summary>
        private readonly ObservableAsPropertyHelper<ValidationText> _validationText;

        /// <summary>
        ///     Subject for validality of the context.
        /// </summary>
        private readonly ReplaySubject<bool> _validSubject = new ReplaySubject<bool>(1);

        private bool _isActive;

        /// <inheritdoc />
        /// <summary>
        ///     Create the context
        /// </summary>
        public ValidationContext()
        {
            // publish the current validation state 
            _disposables.Add(_validSubject.StartWith(true).ToProperty(this, m => m.IsValid, out _isValid));

            // when a change occurs in the validation state, publish the updated validation text
            _disposables.Add(_validSubject.StartWith(true).Select(v => BuildText())
                .ToProperty(this, m => m.Text, out _validationText, new ValidationText()));

            //publish the current validation state
            _disposables.Add(_validSubject.Select(v => new ValidationState(IsValid, BuildText(), this))
                .Do(vc => _validationStatusChange.OnNext(vc)).Subscribe());


            // observe the defined validations and whenever there is a change publish the current validation state.
            _validationConnectable = _validations.CountChanged.StartWith(0)
                .Select(_ =>
                    _validations
                        .Select(v => v.ValidationStatusChange).Merge()
                        .Select(o => Unit.Default).StartWith(Unit.Default))
                .Switch()
                .Select(_ => GetIsValid()).Multicast(_validSubject);
        }

        /// <summary>
        ///     An observable for the Valid state
        /// </summary>
        public IObservable<bool> Valid
        {
            get
            {
                Activate();

                return _validSubject.AsObservable();
            }
        }

        /// <summary>
        ///     Get the list of validations
        /// </summary>
        public IReadOnlyReactiveList<IValidationComponent> Validations => _validations;

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        ///     Get whether currently valid or not
        /// </summary>
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid.Value;
            }
        }

        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validationStatusChange.AsObservable();
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Get the current validation summary.
        /// </summary>
        public ValidationText Text
        {
            get
            {
                Activate();
                return _validationText.Value;
            }
        }

        private void Activate()
        {
            if (_isActive)
                return;

            _isActive = true;
            _disposables.Add(_validationConnectable.Connect());
        }

        public void Add(IValidationComponent validation)
        {
            _validations.Add(validation);
        }

        public bool GetIsValid()
        {
            var isValid = _validations.Count == 0 || _validations.All(v => v.IsValid);
            return isValid;
        }

        /// <summary>
        ///     Build a list of the validation text for each invalid component
        /// </summary>
        /// <returns></returns>
        private ValidationText BuildText()
        {
            return new ValidationText(_validations.Where(p => !p.IsValid).Select(p => p.Text));
        }
    }
}