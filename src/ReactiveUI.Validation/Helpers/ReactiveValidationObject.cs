// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.States;

namespace ReactiveUI.Validation.Helpers
{
    /// <summary>
    /// Base class for ReactiveObjects that support <see cref="INotifyDataErrorInfo"/> validation.
    /// </summary>
    /// <typeparam name="TViewModel">The parent view model.</typeparam>
    [ExcludeFromCodeCoverage]
    [Obsolete("The type parameters are no longer required. Use the non-generic version of ReactiveValidationObject.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Same class just generic.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just generic.")]
    public abstract class ReactiveValidationObject<TViewModel> : ReactiveValidationObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveValidationObject{TViewModel}"/> class.
        /// </summary>
        /// <param name="scheduler">Scheduler for OAPHs and for the the ValidationContext.</param>
        protected ReactiveValidationObject(IScheduler? scheduler = null)
            : base(scheduler)
        {
        }
    }

    /// <summary>
    /// Base class for ReactiveObjects that support <see cref="INotifyDataErrorInfo"/> validation.
    /// </summary>
    public abstract class ReactiveValidationObject : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo
    {
        private readonly HashSet<string> _mentionedPropertyNames = new HashSet<string>();
        private bool _hasErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveValidationObject"/> class.
        /// </summary>
        /// <param name="scheduler">Scheduler for OAPHs and for the the ValidationContext.</param>
        protected ReactiveValidationObject(IScheduler? scheduler = null)
        {
            ValidationContext = new ValidationContext(scheduler);
            ValidationContext.Validations
                .ToObservableChangeSet()
                .ToCollection()
                .Select(components => components
                    .Select(component => component
                        .ValidationStatusChange
                        .Select(_ => component))
                    .Merge()
                    .StartWith(ValidationContext))
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
                    .SelectMany(validation => validation.Text ?? ValidationText.Empty)
                    .ToArray() :
                SelectInvalidPropertyValidations()
                    .Where(validation => validation.ContainsPropertyName(propertyName))
                    .SelectMany(validation => validation.Text ?? ValidationText.Empty)
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
        private IEnumerable<IPropertyValidationComponent> SelectInvalidPropertyValidations() =>
            ValidationContext.Validations
                .OfType<IPropertyValidationComponent>()
                .Where(validation => !validation.IsValid);

        /// <summary>
        /// Updates the <see cref="HasErrors" /> property before raising the <see cref="ErrorsChanged" />
        /// event, and then raises the <see cref="ErrorsChanged" /> event. This behaviour is required by WPF, see:
        /// https://stackoverflow.com/questions/24518520/ui-not-calling-inotifydataerrorinfo-geterrors/24837028.
        /// </summary>
        /// <remarks>
        /// WPF doesn't understand string.Empty as an argument for the <see cref="ErrorsChanged"/>
        /// event, so we are sending <see cref="ErrorsChanged"/> notifications for every saved property.
        /// This is required for e.g. cases when a <see cref="IValidationComponent"/> is disposed and
        /// detached from the <see cref="ValidationContext"/>, and we'd like to mark all invalid
        /// properties as valid (because the thing that validates them no longer exists).
        /// </remarks>
        private void OnValidationStatusChange(IValidationComponent component)
        {
            HasErrors = !ValidationContext.GetIsValid();
            if (component is IPropertyValidationComponent propertyValidationComponent)
            {
                foreach (var propertyName in propertyValidationComponent.Properties)
                {
                    RaiseErrorsChanged(propertyName);
                    _mentionedPropertyNames.Add(propertyName);
                }
            }
            else
            {
                foreach (var propertyName in _mentionedPropertyNames)
                {
                    RaiseErrorsChanged(propertyName);
                }
            }
        }
    }
}
