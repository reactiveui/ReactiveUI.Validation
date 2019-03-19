// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/ModelObservableValidation.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// We probably need a more 'complex' one, where the params of the validation block are
    /// passed through?
    /// Also, what about access to the view model to output the error message?.
    /// </remarks>
    public class ModelObservableValidation<TViewModel> : ReactiveObject, IValidationComponent, IDisposable
    {
        private readonly ReplaySubject<ValidationState> _lastValidationStateSubject =
            new ReplaySubject<ValidationState>(1);

        // the underlying connected observable for the validation change which is published
        private readonly IConnectableObservable<ValidationState> _validityConnectedObservable;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isActive;

        private bool _isValid;

        private ValidationText _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Func to define if the <see cref="viewModel"/> is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the <see cref="viewModel"/> and <see cref="validityObservable"/> values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        private ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, ValidationText> messageFunc)
        {
            _disposables.Add(_lastValidationStateSubject.Do(s =>
            {
                _isValid = s.IsValid;
                _text = s.Text;
            }).Subscribe());

            _validityConnectedObservable = Observable.Defer(() => validityObservable(viewModel))
                .Select(v => new ValidationState(v, messageFunc(viewModel, v), this))
                .Multicast(_lastValidationStateSubject);
        }

        /// <inheritdoc/>
        public ValidationText Text
        {
            get
            {
                Activate();
                return _text;
            }
        }

        /// <inheritdoc/>
        public bool IsValid
        {
            get
            {
                Activate();
                return _isValid;
            }
        }

        /// <inheritdoc/>
        public IObservable<ValidationState> ValidationStatusChange
        {
            get
            {
                Activate();
                return _validityConnectedObservable;
            }
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
            _disposables.Add(_validityConnectedObservable.Connect());
        }
    }
}