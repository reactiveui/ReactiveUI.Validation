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
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// A validation component that is based on an <see cref="IObservable{T}"/>. Validates a single property.
    /// Though in the passed observable more properties can be referenced via a call to WhenAnyValue.
    /// </summary>
    public sealed class ObservableValidation<TViewModel, TValue, TProp> : ObservableValidation<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message as a constant.</param>
        public ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TValue, bool> isValidFunc,
            string message)
            : this(viewModel, viewModelProperty, observable, (mv, state) => isValidFunc(state), (vm, state) => message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message as a constant.</param>
        public ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            string message)
            : this(viewModel, viewModelProperty, observable, isValidFunc, (vm, state) => message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
            : this(viewModel, viewModelProperty, observable, (vm, state) => isValidFunc(state), (vm, state) =>
                messageFunc(state))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, string> messageFunc)
            : this(viewModel, viewModelProperty, observable, isValidFunc, (vm, value, isValid) =>
                isValid ? string.Empty : messageFunc(vm, value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, bool, string> messageFunc)
            : this(viewModel, viewModelProperty, observable, isValidFunc, (vm, value, isValid) =>
                new ValidationText(messageFunc(vm, value, isValid)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        private ObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, bool, ValidationText> messageFunc)
            : base(observable.Select(value =>
            {
                var isValid = isValidFunc(viewModel, value);
                var message = messageFunc(viewModel, value, isValid);
                return new ValidationState(isValid, message);
            })) =>
            AddProperty(viewModelProperty);
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// A validation component that is based on an <see cref="IObservable{T}"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
    public sealed class ObservableValidation<TViewModel, TValue> : ObservableValidation<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message as a constant.</param>
        public ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TValue, bool> isValidFunc,
            string message)
            : this(viewModel, observable, (mv, state) => isValidFunc(state), (vm, state) => message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message as a constant.</param>
        public ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            string message)
            : this(viewModel, observable, isValidFunc, (vm, state) => message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
            : this(viewModel, observable, (vm, state) => isValidFunc(state), (vm, state) => messageFunc(state))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, string> messageFunc)
            : this(viewModel, observable, isValidFunc, (vm, value, isValid) =>
                isValid ? string.Empty : messageFunc(vm, value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        public ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, bool, string> messageFunc)
            : this(viewModel, observable, isValidFunc, (vm, value, isValid) =>
                new ValidationText(messageFunc(vm, value, isValid)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        private ObservableValidation(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, bool, ValidationText> messageFunc)
            : base(observable.Select(value =>
            {
                var isValid = isValidFunc(viewModel, value);
                var message = messageFunc(viewModel, value, isValid);
                return new ValidationState(isValid, message);
            }))
        {
        }
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// A validation component that is based on an <see cref="IObservable{T}"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Obsolete("This class is going to be removed in future versions. Consider using ObservableValidation<TViewModel> as a base class.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
    public abstract class ObservableValidationBase<TViewModel, TValue> : ObservableValidation<TViewModel>, IPropertyValidationComponent<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidationBase{TViewModel,TValue}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        /// <param name="isValidFunc">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message.</param>
        [ExcludeFromCodeCoverage]
        [Obsolete("This class is going to be removed in future versions. Consider using ObservableValidation<TViewModel> as a base class.")]
        protected ObservableValidationBase(
            TViewModel viewModel,
            IObservable<TValue> observable,
            Func<TViewModel, TValue, bool> isValidFunc,
            Func<TViewModel, TValue, bool, ValidationText> messageFunc)
            : base(observable.Select(value =>
            {
                var isValid = isValidFunc(viewModel, value);
                var message = messageFunc(viewModel, value, isValid);
                return new ValidationState(isValid, message);
            }))
        {
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
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// A validation component that is based on an <see cref="IObservable{T}"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
    public class ObservableValidation<TViewModel> : ReactiveObject, IDisposable, IPropertyValidationComponent
    {
        [SuppressMessage("Usage", "CA2213:Disposable fields should be disposed", Justification = "Disposed by field _disposables.")]
        private readonly ReplaySubject<IValidationState> _isValidSubject = new ReplaySubject<IValidationState>(1);
        private readonly HashSet<string> _propertyNames = new HashSet<string>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly IConnectableObservable<IValidationState> _validityConnectedObservable;
        private bool _isActive;
        private bool _isValid;
        private ValidationText? _text;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="observable">Observable that updates the view model property validity.</param>
        public ObservableValidation(IObservable<IValidationState> observable)
        {
            _isValidSubject
                .Do(state =>
                {
                    _isValid = state.IsValid;
                    _text = state.Text;
                })
                .Subscribe()
                .DisposeWith(_disposables);

            _validityConnectedObservable = Observable
                .Defer(() => observable)
                .Multicast(_isValidSubject);
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
        public IObservable<IValidationState> ValidationStatusChange
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
        public bool ContainsPropertyName(string propertyName, bool exclusively = false) =>
            exclusively
                ? _propertyNames.Contains(propertyName) &&
                  _propertyNames.Count == 1
                : _propertyNames.Contains(propertyName);

        /// <summary>
        /// Adds a property to the list of properties which this validation is associated with.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="property">ViewModel property.</param>
        public void AddProperty<TProp>(Expression<Func<TViewModel, TProp>> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var propertyName = property.Body.GetPropertyPath();
            _propertyNames.Add(propertyName);
        }

        /// <summary>
        /// Disposes of the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// If its getting called by the <see cref="BasePropertyValidation{TViewModel}.Dispose()"/> method.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.Dispose();
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
