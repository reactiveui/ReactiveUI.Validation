// <copyright file="ReactiveValidationObject.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Helpers
{
    /// <summary>
    /// Base class for ReactiveObjects that support INotifyDataErrorInfo validation.
    /// </summary>
    /// <typeparam name="TViewModel">The parent view model.</typeparam>
    public abstract class ReactiveValidationObject<TViewModel> : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo
    {
        private readonly ObservableAsPropertyHelper<bool> _hasErrors;
        private readonly Dictionary<string, string> propertyMemberNameDictionary = new Dictionary<string, string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveValidationObject{TViewModel}"/> class.
        /// </summary>
        /// <param name="scheduler">Scheduler for OAPHs and for the the ValidationContext.</param>
        protected ReactiveValidationObject(IScheduler scheduler = null)
        {
            ValidationContext = new ValidationContext(scheduler);

            _hasErrors = this
                .IsValid()
                .Select(valid => !valid)
                .ToProperty(this, x => x.HasErrors, scheduler: scheduler);

            ValidationContext.ValidationStatusChange
                .Select(validity => new DataErrorsChangedEventArgs(string.Empty))
                .Subscribe(args => ErrorsChanged?.Invoke(this, args));
        }

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <inheritdoc />
        public bool HasErrors => _hasErrors.Value;

        /// <inheritdoc />
        public ValidationContext ValidationContext { get; }

        /// <summary>
        /// Returns a collection of error messages, required by the INotifyDataErrorInfo interface.
        /// </summary>
        /// <param name="propertyName">Property to search error notifications for.</param>
        /// <returns>A list of error messages, usually strings.</returns>
        /// <inheritdoc />
        public virtual IEnumerable GetErrors(string propertyName)
        {
            return string.IsNullOrEmpty(propertyName) ?
                SelectValidations()
                    .SelectMany(validation => validation.Text)
                    .ToArray() :
                GetMemberInfoName(propertyName) is string memberInfoName && string.IsNullOrEmpty(memberInfoName) == false ?
                    SelectValidations()
                        .Where(validation => validation.ContainsPropertyName(memberInfoName))
                        .SelectMany(validation => validation.Text)
                        .ToArray() :
                    Enumerable.Empty<string>();

            IEnumerable<IPropertyValidationComponent<TViewModel>> SelectValidations() =>

                ValidationContext
                    .Validations
                    .OfType<IPropertyValidationComponent<TViewModel>>()
                    .Where(validation => !validation.IsValid);
        }

        public string GetMemberInfoName(string propertyName)
        {
            if (propertyMemberNameDictionary.ContainsKey(propertyName) == false)
            {
                propertyMemberNameDictionary.Add(propertyName, GetMemberInfoName());
            }

            return propertyMemberNameDictionary[propertyName];

            string GetMemberInfoName() => GetType()
                .GetMember(propertyName)
                .FirstOrDefault()?
                .ToString();
        }
    }
}
