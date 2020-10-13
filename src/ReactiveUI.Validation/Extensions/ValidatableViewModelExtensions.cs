// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods associated to <see cref="IValidatableViewModel"/> instances.
    /// </summary>
    public static class ValidatableViewModelExtensions
    {
        /// <summary>
        /// Setup a validation rule for a specified ViewModel property with static error message.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModelProp, bool> isPropertyValid,
            string message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (isPropertyValid is null)
            {
                throw new ArgumentNullException(nameof(isPropertyValid));
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            // We need to associate the ViewModel property with
            // something that can be easily looked up and bound to.
            return viewModel.RegisterValidation(
                new BasePropertyValidation<TViewModel, TViewModelProp>(
                    viewModel, viewModelProperty, isPropertyValid, message));
        }

        /// <summary>
        /// Setup a validation rule for a specified ViewModel property with dynamic error message.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModelProp, bool> isPropertyValid,
            Func<TViewModelProp, string> message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (isPropertyValid is null)
            {
                throw new ArgumentNullException(nameof(isPropertyValid));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return viewModel.RegisterValidation(
                new BasePropertyValidation<TViewModel, TViewModelProp>(
                    viewModel, viewModelProperty, isPropertyValid, message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity and a static error message.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        public static ValidationHelper ValidationRule<TViewModel>(
            this TViewModel viewModel,
            IObservable<bool> validationObservable,
            string message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (validationObservable is null)
            {
                throw new ArgumentNullException(nameof(validationObservable));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    viewModel,
                    validationObservable,
                    validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity with a dynamic
        /// validation function and a dynamic context-aware error message.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        public static ValidationHelper ValidationRule<TViewModel, TValue>(
            this TViewModel viewModel,
            IObservable<TValue> validationObservable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (validationObservable is null)
            {
                throw new ArgumentNullException(nameof(validationObservable));
            }

            if (isValidFunc is null)
            {
                throw new ArgumentNullException(nameof(isValidFunc));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, TValue>(
                    viewModel, validationObservable, isValidFunc, messageFunc));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity and a static error message
        /// for the given view model property.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<bool> viewModelObservable,
            string message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewModelObservable is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservable));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservable,
                    validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity with a dynamic
        /// validation function and a dynamic context-aware error message for the given view model property.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp, TValue>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<TValue> viewModelObservable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewModelObservable is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservable));
            }

            if (isValidFunc is null)
            {
                throw new ArgumentNullException(nameof(isValidFunc));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, TValue, TViewModelProp>(
                    viewModel, viewModelProperty, viewModelObservable, isValidFunc, messageFunc));
        }

        /// <summary>
        /// Gets an observable for the validity of the ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <returns>Returns true if the ValidationContext is valid, otherwise false.</returns>
        public static IObservable<bool> IsValid<TViewModel>(this TViewModel viewModel)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            return viewModel.ValidationContext.Valid;
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use the overload accepting just IObservable<bool> instead of Func<TViewModel, IObservable<bool>>")]
        public static ValidationHelper ValidationRule<TViewModel>(
            this TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            string message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    viewModel,
                    viewModelObservableProperty(viewModel),
                    validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and viewModelObservableProperty values.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use the overload accepting just IObservable<bool> instead of Func<TViewModel, IObservable<bool>>")]
        public static ValidationHelper ValidationRule<TViewModel>(
            this TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            Func<TViewModel, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    viewModel,
                    viewModelObservableProperty(viewModel),
                    (vm, state) => state,
                    (vm, state) => messageFunc(vm)));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and viewModelObservableProperty values.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("This overload is planned for future removal. Consider using either the overload that accepts a " +
                  "Func<TViewModel, string> as the messageFunc parameter, or the overload that accepts a string.")]
        public static ValidationHelper ValidationRule<TViewModel>(
            this TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            Func<TViewModel, bool, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    viewModel,
                    viewModelObservableProperty(viewModel),
                    (vm, validity) => validity,
                    (vm, state, isValid) => messageFunc(vm, isValid)));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use the overload accepting just IObservable<bool> instead of Func<TViewModel, IObservable<bool>>")]
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            string message)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservableProperty(viewModel),
                    validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and viewModelObservableProperty values.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use the overload accepting just IObservable<bool> instead of Func<TViewModel, IObservable<bool>>")]
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            Func<TViewModel, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservableProperty(viewModel),
                    (vm, validity) => validity,
                    (vm, state) => messageFunc(vm)));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservableProperty">Func to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and viewModelObservableProperty values.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        [Obsolete("This overload is planned for future removal. Consider using either the overload that accepts a " +
                  "Func<TViewModel, string> as the messageFunc parameter, or the overload that accepts a string.")]
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(
            this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> viewModelObservableProperty,
            Func<TViewModel, bool, string> messageFunc)
            where TViewModel : IReactiveObject, IValidatableViewModel
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModelProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelProperty));
            }

            if (viewModelObservableProperty is null)
            {
                throw new ArgumentNullException(nameof(viewModelObservableProperty));
            }

            if (messageFunc is null)
            {
                throw new ArgumentNullException(nameof(messageFunc));
            }

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservableProperty(viewModel),
                    (vm, validity) => validity,
                    (vm, state, validity) => messageFunc(vm, validity)));
        }

        /// <summary>
        /// Registers an <see cref="IValidationComponent"/> into the <see cref="ValidationContext"/>
        /// of the specified <see cref="IValidatableViewModel"/>. Disposes and removes the
        /// <see cref="IValidationComponent"/> from the <see cref="ValidationContext"/> when the
        /// <see cref="ValidationHelper"/> is disposed.
        /// </summary>
        /// <param name="viewModel">The validatable view model holding a reference to the context.</param>
        /// <param name="validation">The disposable validation component to register into the context.</param>
        /// <typeparam name="TValidationComponent">The disposable validation component type.</typeparam>
        /// <returns>The bindable validation helper holding the disposable.</returns>
        private static ValidationHelper RegisterValidation<TValidationComponent>(
            this IValidatableViewModel viewModel,
            TValidationComponent validation)
            where TValidationComponent : IValidationComponent, IDisposable
        {
            viewModel.ValidationContext.Add(validation);
            return new ValidationHelper(validation, Disposable.Create(() =>
            {
                viewModel.ValidationContext.Remove(validation);
                validation.Dispose();
            }));
        }
    }
}
