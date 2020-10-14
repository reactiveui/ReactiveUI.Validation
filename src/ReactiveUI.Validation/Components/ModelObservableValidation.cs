// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Extensions;
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
    /// Validates a single property. Though in the passed validityObservable more properties can be referenced.
    /// We probably need a more 'complex' one, where the params of the validation block are
    /// passed through?
    /// Also, what about access to the view model to output the error message?.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [Obsolete("Consider using ObservableValidation<TViewModel, bool, TViewModelProp> instead.")]
    public class ModelObservableValidation<TViewModel, TViewModelProp> : ModelObservableValidationBase<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            string message)
            : this(viewModel, viewModelProperty, validityObservable, (p, isValid) =>
                new ValidationText(isValid ? string.Empty : message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, string> message)
            : this(viewModel, viewModelProperty, validityObservable, (p, isValid) =>
                new ValidationText(isValid ? string.Empty : message(p)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, viewModelProperty, validityObservable, (vm, state) =>
                new ValidationText(messageFunc(vm, state)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        private ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, ValidationText> messageFunc)
            : base(viewModel, validityObservable, messageFunc)
        {
            // record this property name
            AddProperty(viewModelProperty);
        }
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// for backwards compatibility, validated properties are not explicitly defined, so we don't really know what's inside the validityObservable.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [Obsolete("Consider using ObservableValidation<TViewModel, bool> instead.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
    public class ModelObservableValidation<TViewModel> : ModelObservableValidationBase<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            string message)
            : this(viewModel, validityObservable, (p, isValid) =>
                new ValidationText(isValid ? string.Empty : message))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, string> message)
            : this(viewModel, validityObservable, (p, isValid) =>
                new ValidationText(isValid ? string.Empty : message(p)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        private ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, ValidationText> messageFunc)
            : base(viewModel, validityObservable, messageFunc)
        {
        }
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// We probably need a more 'complex' one, where the params of the validation block are
    /// passed through?
    /// Also, what about access to the view model to output the error message?.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [Obsolete("Consider using ObservableValidation<TViewModel, bool> instead.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just an abstract one.")]
    public abstract class ModelObservableValidationBase<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent, IPropertyValidationComponent<TViewModel>
    {
        [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by field _disposables.")]
        private readonly ReplaySubject<ValidationState> _lastValidationStateSubject = new ReplaySubject<ValidationState>(1);
        private readonly HashSet<string> _propertyNames = new HashSet<string>();
        private readonly IConnectableObservable<ValidationState> _validityConnectedObservable;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isActive;
        private bool _isValid;
        private ValidationText? _text;

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
            _lastValidationStateSubject
                .Do(state =>
                {
                    _isValid = state.IsValid;
                    _text = state.Text;
                })
                .Subscribe()
                .DisposeWith(_disposables);

            _validityConnectedObservable = Observable
                .Defer(() => validityObservable(viewModel))
                .Select(v => new ValidationState(v, messageFunc(viewModel, v), this))
                .Multicast(_lastValidationStateSubject);
        }

        /// <inheritdoc/>
        public int PropertyCount => _propertyNames.Count;

        /// <inheritdoc/>
        public IEnumerable<string> Properties => _propertyNames.AsEnumerable();

        /// <inheritdoc/>
        public ValidationText? Text
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
        [ExcludeFromCodeCoverage]
        [Obsolete("Consider using the non-generic ContainsProperty of a non-generic IPropertyValidationComponent.")]
        public bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> property, bool exclusively = false)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyName = property.Body.GetPropertyPath();
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
                _disposables.Dispose();
            }
        }

        /// <summary>
        /// Adds a property to the list of this which this validation is associated with.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="property">ViewModel property.</param>
        protected void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyName = property.Body.GetPropertyPath();
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
