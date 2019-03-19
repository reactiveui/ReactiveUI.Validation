// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Contexts/ValidationContext.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

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
    /// The overall context for a view model under which validation takes place.
    /// </summary>
    /// <remarks>
    /// Contains all of the <see cref="ReactiveUI.Validation.Components.Abstractions.IValidationComponent" /> instances
    /// applicable to the view model.
    /// </remarks>
    public class ValidationContext : ReactiveObject, IDisposable, IValidationComponent
    {
        /// <summary>
        /// Backing field for the current validation state.
        /// </summary>
        private readonly ObservableAsPropertyHelper<bool> _isValid;

        private readonly IConnectableObservable<bool> _validationConnectable;

        /// <summary>
        /// The list of current validations.
        /// </summary>
        private readonly ReactiveList<IValidationComponent> _validations = new ReactiveList<IValidationComponent>();

        private readonly ReplaySubject<ValidationState> _validationStatusChange = new ReplaySubject<ValidationState>(1);

        /// <summary>
        /// Backing field for the validation summary.
        /// </summary>
        private readonly ObservableAsPropertyHelper<ValidationText> _validationText;

        /// <summary>
        /// Subject for validity of the context.
        /// </summary>
        private readonly ReplaySubject<bool> _validSubject = new ReplaySubject<bool>(1);

        private CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isActive;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        public ValidationContext()
        {
            // Publish the current validation state
            _disposables.Add(_validSubject.StartWith(true).ToProperty(this, m => m.IsValid, out _isValid));

            // When a change occurs in the validation state, publish the updated validation text
            _disposables.Add(_validSubject.StartWith(true).Select(v => BuildText())
                .ToProperty(this, m => m.Text, out _validationText, new ValidationText()));

            // Publish the current validation state
            _disposables.Add(_validSubject.Select(v => new ValidationState(IsValid, BuildText(), this))
                .Do(vc => _validationStatusChange.OnNext(vc)).Subscribe());

            // Observe the defined validations and whenever there is a change publish the current validation state.
            _validationConnectable = _validations.CountChanged.StartWith(0)
                .Select(_ =>
                    _validations
                        .Select(v => v.ValidationStatusChange).Merge()
                        .Select(o => Unit.Default).StartWith(Unit.Default))
                .Switch()
                .Select(_ => GetIsValid()).Multicast(_validSubject);
        }

        /// <summary>
        /// Gets an observable for the Valid state.
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
        /// Gets get the list of validations.
        /// </summary>
        public IReadOnlyReactiveList<IValidationComponent> Validations => _validations;

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid.Value;
            }
        }

        /// <inheritdoc />
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validationStatusChange.AsObservable();
            }
        }

        /// <inheritdoc />
        public ValidationText Text
        {
            get
            {
                Activate();
                return _validationText.Value;
            }
        }

        /// <summary>
        /// Adds a validation into the validations collection.
        /// </summary>
        /// <param name="validation">Validation component to be added into the collection.</param>
        public void Add(IValidationComponent validation)
        {
            _validations.Add(validation);
        }

        /// <summary>
        /// Returns if the whole context is valid checking all the validations.
        /// </summary>
        /// <returns>Returns true if the <see cref="ValidationContext"/> is valid, otherwise false.</returns>
        public bool GetIsValid()
        {
            var isValid = _validations.Count == 0 || _validations.All(v => v.IsValid);
            return isValid;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="Dispose"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        private void Activate()
        {
            if (_isActive)
            {
                return;
            }

            _isActive = true;
            _disposables.Add(_validationConnectable.Connect());
        }

        /// <summary>
        /// Build a list of the validation text for each invalid component.
        /// </summary>
        /// <returns>Returns the <see cref="ValidationText"/> with all the error messages from the non valid components.</returns>
        private ValidationText BuildText()
        {
            return new ValidationText(_validations.Where(p => !p.IsValid).Select(p => p.Text));
        }
    }
}