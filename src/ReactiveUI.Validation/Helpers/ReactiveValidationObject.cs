// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers
{
    /// <summary>
    /// Base class for ReactiveObjects that support INotifyDataErrorInfo validation.
    /// </summary>
    /// <typeparam name="TViewModel">The parent view model.</typeparam>
    public abstract class ReactiveValidationObject<TViewModel> : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo
    {
        private bool _hasErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveValidationObject{TViewModel}"/> class.
        /// </summary>
        /// <param name="scheduler">Scheduler for OAPHs and for the the ValidationContext.</param>
        protected ReactiveValidationObject(IScheduler? scheduler = null)
        {
            ValidationContext = new ValidationContext(scheduler);
            ValidationContext.Validations
                .ToObservableChangeSet()
                .ToCollection()
                .Select(components => components
                    .Select(component => component.ValidationStatusChange)
                    .Merge())
                .Switch()
                .Subscribe(OnValidationStatusChange);
        }

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <inheritdoc />
        public bool HasErrors
        {
            get => _hasErrors;
            private set => this.RaiseAndSetIfChanged(ref _hasErrors, value);
        }

        /// <inheritdoc />
        public ValidationContext ValidationContext { get; }

        /// <summary>
        /// Returns a collection of error messages, required by the INotifyDataErrorInfo interface.
        /// </summary>
        /// <param name="propertyName">Property to search error notifications for.</param>
        /// <returns>A list of error messages, usually strings.</returns>
        /// <inheritdoc />
        public virtual IEnumerable GetErrors(string propertyName) =>
            string.IsNullOrEmpty(propertyName) ?
                SelectInvalidPropertyValidations()
                    .SelectMany(validation => validation.Text)
                    .ToArray() :
                SelectInvalidPropertyValidations()
                    .Where(validation => validation.ContainsPropertyName(propertyName))
                    .SelectMany(validation => validation.Text)
                    .ToArray();

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the validated property.</param>
        protected void RaiseErrorsChanged(string propertyName = "") =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        /// <summary>
        /// Selects validation components that are invalid.
        /// </summary>
        /// <returns>Returns the invalid property validations.</returns>
        private IEnumerable<IPropertyValidationComponent<TViewModel>> SelectInvalidPropertyValidations() =>
            ValidationContext.Validations
                .OfType<IPropertyValidationComponent<TViewModel>>()
                .Where(validation => !validation.IsValid);

        /// <summary>
        /// Updates the <see cref="HasErrors" /> property before raising the <see cref="ErrorsChanged" />
        /// event, and then raises the <see cref="ErrorsChanged" /> event. This behaviour is required by WPF, see:
        /// https://stackoverflow.com/questions/24518520/ui-not-calling-inotifydataerrorinfo-geterrors/24837028.
        /// </summary>
        private void OnValidationStatusChange(ValidationState state)
        {
            HasErrors = !ValidationContext.GetIsValid();
            if (state.Component is IPropertyValidationComponent<TViewModel> propertyValidationComponent &&
                propertyValidationComponent.PropertyCount == 1)
            {
                var propertyName = propertyValidationComponent.Properties.First();
                RaiseErrorsChanged(propertyName);
            }
            else
            {
                RaiseErrorsChanged();
            }
        }
    }
}
