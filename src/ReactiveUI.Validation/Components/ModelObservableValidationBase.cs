// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/ModelObservableValidationBase.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Components
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IPropertyValidationComponent{TViewModel}" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// We probably need a more 'complex' one, where the params of the validation block are
    /// passed through?
    /// Also, what about access to the view model to output the error message?.
    /// </remarks>
    public abstract class ModelObservableValidationBase<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent<TViewModel>
    {
        private readonly ReplaySubject<ValidationState> _lastValidationStateSubject =
            new ReplaySubject<ValidationState>(1);

        /// <summary>
        /// The list of property names this validator is referencing.
        /// </summary>
        private readonly HashSet<string> _propertyNames = new HashSet<string>();

        // the underlying connected observable for the validation change which is published
        private readonly IConnectableObservable<ValidationState> _validityConnectedObservable;

        private CompositeDisposable _disposables = new CompositeDisposable();

        private bool _isActive;

        private bool _isValid;

        private ValidationText _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidationBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidationBase(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidationBase{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        protected ModelObservableValidationBase(
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
        public int PropertyCount => _propertyNames.Count;

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

        /// <inheritdoc/>
        public bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> property, bool exclusively = false)
        {
            var propertyName = property.Body.GetMemberInfo().ToString();
            return ContainsPropertyName(propertyName, exclusively);
        }

        /// <inheritdoc/>
        public bool ContainsPropertyName(string propertyName, bool exclusively = false)
        {
            return exclusively
                ? _propertyNames.Contains(propertyName) && _propertyNames.Count == 1
                : _propertyNames.Contains(propertyName);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Adds a property to the list of this which this validation is associated with.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="property">ViewModel property.</param>
        protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
        {
            var propertyName = property.Body.GetMemberInfo().ToString();
            _propertyNames.Add(propertyName);
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
