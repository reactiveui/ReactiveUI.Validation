// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/TemplateGenerators/PropertyValidationGenerator.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.TemplateGenerators
{
    /// <inheritdoc />
    [SuppressMessage("IDE", "SA1649", Justification = "Generated classes with template.")]
    [SuppressMessage("IDE", "SA1402", Justification = "Generated classes with template.")]
    public sealed class BasePropertyValidation<TViewModel, TProperty1, TProperty2>
        : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1, TProperty2>> _valueSubject = new Subject<Tuple<TProperty1, TProperty2>>();

        /// <summary>
        /// The validation message factory.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2>, bool, ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated.
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1, TProperty2>> _valueConnectedObservable;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2>, bool> _isValidFunc;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1, TProperty2> _lastValue;

        /// <summary>
        /// Are we connected.
        /// </summary>
        private bool _connected;

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Func<Tuple<TProperty1, TProperty2>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2>, string> message)
            : this(viewModel, property1, property2, isValidFunc, (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Func<Tuple<TProperty1, TProperty2>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2>, bool, string> messageFunc)
            : this(viewModel, property1, property2, isValidFunc, (p, v) => new ValidationText(messageFunc(p, v)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Func<Tuple<TProperty1, TProperty2>, bool> validSelector,
            Func<Tuple<TProperty1, TProperty2>, bool, ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // Add the properties used to our list
            AddProperty(property1);
            AddProperty(property2);
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // Setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2)
                .DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <inheritdoc/>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, this.GetMessage(value, isValid), this);
            }).DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        /// <param name="params">ViewModel properties.</param>
        /// <param name="isValid">Whether the property is valid or not.</param>
        /// <returns>Returns the <see cref="ValidationText"/> object.</returns>
        private ValidationText GetMessage(Tuple<TProperty1, TProperty2> @params, bool isValid)
        {
            return _message(@params, isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _connected = true;
                _disposables.Add(_valueConnectedObservable.Connect());
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("IDE", "SA1649", Justification = "Generated classes with template.")]
    [SuppressMessage("IDE", "SA1402", Justification = "Generated classes with template.")]
    public sealed class BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>
        : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1, TProperty2, TProperty3>> _valueSubject = new Subject<Tuple<TProperty1, TProperty2, TProperty3>>();

        /// <summary>
        /// The validation message factory.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3>, bool, ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated.
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1, TProperty2, TProperty3>> _valueConnectedObservable;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3>, bool> _isValidFunc;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1, TProperty2, TProperty3> _lastValue;

        /// <summary>
        /// Are we connected.
        /// </summary>
        private bool _connected;

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, string> message)
            : this(viewModel, property1, property2, property3, isValidFunc, (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, bool, string> messageFunc)
            : this(viewModel, property1, property2, property3, isValidFunc, (p, v) => new ValidationText(messageFunc(p, v)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, bool> validSelector,
            Func<Tuple<TProperty1, TProperty2, TProperty3>, bool, ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // Add the properties used to our list
            AddProperty(property1);
            AddProperty(property2);
            AddProperty(property3);
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // Setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3)
                .DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <inheritdoc/>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, this.GetMessage(value, isValid), this);
            }).DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        /// <param name="params">ViewModel properties.</param>
        /// <param name="isValid">Whether the property is valid or not.</param>
        /// <returns>Returns the <see cref="ValidationText"/> object.</returns>
        private ValidationText GetMessage(Tuple<TProperty1, TProperty2, TProperty3> @params, bool isValid)
        {
            return _message(@params, isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _connected = true;
                _disposables.Add(_valueConnectedObservable.Connect());
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("IDE", "SA1649", Justification = "Generated classes with template.")]
    [SuppressMessage("IDE", "SA1402", Justification = "Generated classes with template.")]
    public sealed class BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3, TProperty4>
        : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>> _valueSubject = new Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>>();

        /// <summary>
        /// The validation message factory.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool, ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated.
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>> _valueConnectedObservable;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool> _isValidFunc;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1, TProperty2, TProperty3, TProperty4> _lastValue;

        /// <summary>
        /// Are we connected.
        /// </summary>
        private bool _connected;

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, string> message)
            : this(viewModel, property1, property2, property3, property4, isValidFunc, (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool, string> messageFunc)
            : this(viewModel, property1, property2, property3, property4, isValidFunc, (p, v) => new ValidationText(messageFunc(p, v)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool> validSelector,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4>, bool, ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // Add the properties used to our list
            AddProperty(property1);
            AddProperty(property2);
            AddProperty(property3);
            AddProperty(property4);
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // Setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4)
                .DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <inheritdoc/>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, this.GetMessage(value, isValid), this);
            }).DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        /// <param name="params">ViewModel properties.</param>
        /// <param name="isValid">Whether the property is valid or not.</param>
        /// <returns>Returns the <see cref="ValidationText"/> object.</returns>
        private ValidationText GetMessage(Tuple<TProperty1, TProperty2, TProperty3, TProperty4> @params, bool isValid)
        {
            return _message(@params, isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _connected = true;
                _disposables.Add(_valueConnectedObservable.Connect());
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("IDE", "SA1649", Justification = "Generated classes with template.")]
    [SuppressMessage("IDE", "SA1402", Justification = "Generated classes with template.")]
    public sealed class BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>
        : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>> _valueSubject = new Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>>();

        /// <summary>
        /// The validation message factory.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool, ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated.
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>> _valueConnectedObservable;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool> _isValidFunc;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5> _lastValue;

        /// <summary>
        /// Are we connected.
        /// </summary>
        private bool _connected;

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, string> message)
            : this(viewModel, property1, property2, property3, property4, property5, isValidFunc, (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool, string> messageFunc)
            : this(viewModel, property1, property2, property3, property4, property5, isValidFunc, (p, v) => new ValidationText(messageFunc(p, v)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool> validSelector,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5>, bool, ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // Add the properties used to our list
            AddProperty(property1);
            AddProperty(property2);
            AddProperty(property3);
            AddProperty(property4);
            AddProperty(property5);
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // Setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4, property5)
                .DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <inheritdoc/>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, this.GetMessage(value, isValid), this);
            }).DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        /// <param name="params">ViewModel properties.</param>
        /// <param name="isValid">Whether the property is valid or not.</param>
        /// <returns>Returns the <see cref="ValidationText"/> object.</returns>
        private ValidationText GetMessage(Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5> @params, bool isValid)
        {
            return _message(@params, isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _connected = true;
                _disposables.Add(_valueConnectedObservable.Connect());
            }
        }
    }

    /// <inheritdoc />
    [SuppressMessage("IDE", "SA1649", Justification = "Generated classes with template.")]
    [SuppressMessage("IDE", "SA1402", Justification = "Generated classes with template.")]
    public sealed class BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>
        : BasePropertyValidation<TViewModel>
    {
        /// <summary>
        /// Represents the current value.
        /// </summary>
        private readonly Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>> _valueSubject = new Subject<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>>();

        /// <summary>
        /// The validation message factory.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool, ValidationText> _message;

        /// <summary>
        /// The connected observable to see updates in properties being validated.
        /// </summary>
        private readonly IConnectableObservable<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>> _valueConnectedObservable;

        /// <summary>
        /// Function to determine if valid or not.
        /// </summary>
        private readonly Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool> _isValidFunc;

        private CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// The last calculated value of the properties.
        /// </summary>
        private Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6> _lastValue;

        /// <summary>
        /// Are we connected.
        /// </summary>
        private bool _connected;

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Expression<Func<TViewModel, TProperty6>> property6,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, string> message)
            : this(viewModel, property1, property2, property3, property4, property5, property6, isValidFunc, (p, v) => new ValidationText(v ? string.Empty : message(p)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Expression<Func<TViewModel, TProperty6>> property6,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool> isValidFunc,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool, string> messageFunc)
            : this(viewModel, property1, property2, property3, property4, property5, property6, isValidFunc, (p, v) => new ValidationText(messageFunc(p, v)))
        {
        }

        /// <inheritdoc />
        public BasePropertyValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TProperty1>> property1,
            Expression<Func<TViewModel, TProperty2>> property2,
            Expression<Func<TViewModel, TProperty3>> property3,
            Expression<Func<TViewModel, TProperty4>> property4,
            Expression<Func<TViewModel, TProperty5>> property5,
            Expression<Func<TViewModel, TProperty6>> property6,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool> validSelector,
            Func<Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6>, bool, ValidationText> message)
        {
            _message = message;
            _isValidFunc = validSelector;

            // Add the properties used to our list
            AddProperty(property1);
            AddProperty(property2);
            AddProperty(property3);
            AddProperty(property4);
            AddProperty(property5);
            AddProperty(property6);
            _disposables.Add(_valueSubject.Subscribe(v => _lastValue = v));

            // Setup a connected observable to see when values change and cast that to our value subject
            _valueConnectedObservable = viewModel.WhenAnyValue(property1, property2, property3, property4, property5, property6)
                .DistinctUntilChanged()
                .Multicast(_valueSubject);
        }

        /// <inheritdoc/>
        protected override IObservable<ValidationState> GetValidationChangeObservable()
        {
            Activate();

            return _valueSubject.Select(value =>
            {
                var isValid = _isValidFunc(value);
                return new ValidationState(isValid, this.GetMessage(value, isValid), this);
            }).DistinctUntilChanged(new ValidationStateComparer());
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables?.Dispose();
                _disposables = null;
            }
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        /// <param name="params">ViewModel properties.</param>
        /// <param name="isValid">Whether the property is valid or not.</param>
        /// <returns>Returns the <see cref="ValidationText"/> object.</returns>
        private ValidationText GetMessage(Tuple<TProperty1, TProperty2, TProperty3, TProperty4, TProperty5, TProperty6> @params, bool isValid)
        {
            return _message(@params, isValid);
        }

        /// <summary>
        /// Activate the connection to ensure we start seeing validations.
        /// </summary>
        private void Activate()
        {
            if (!_connected)
            {
                _connected = true;
                _disposables.Add(_valueConnectedObservable.Connect());
            }
        }
    }
}