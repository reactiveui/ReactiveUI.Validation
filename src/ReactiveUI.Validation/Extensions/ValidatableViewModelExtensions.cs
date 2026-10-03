// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Extensions methods associated to <see cref="IValidatableViewModel"/> instances.</summary>
public static class ValidatableViewModelExtensions
{
    /// <summary>Provides RegisterValidation extension members for <paramref name="viewModel"/>.</summary>
    /// <param name="viewModel">The view model that owns the validation context.</param>
    extension(IValidatableViewModel viewModel)
    {
        /// <summary>
        /// Registers an <see cref="IValidationComponent"/> into the <see cref="ValidationContext"/>
        /// of the specified <see cref="IValidatableViewModel"/>. Disposes and removes the
        /// <see cref="IValidationComponent"/> from the <see cref="ValidationContext"/> when the
        /// <see cref="ValidationHelper"/> is disposed.
        /// </summary>
        /// <typeparam name="TValidationComponent">The disposable validation component type.</typeparam>
        /// <param name="validation">The disposable validation component to register into the context.</param>
        /// <returns>The bindable validation helper holding the disposable.</returns>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        internal ValidationHelper RegisterValidation<TValidationComponent>(
            TValidationComponent validation)
            where TValidationComponent : IValidationComponent, IDisposable
        {
            viewModel.ValidationContext.Add(validation);
            return new(validation, Disposable.Create(
                (viewModel, validation),
                static state =>
                {
                    state.viewModel.ValidationContext?.Remove(state.validation);
                    state.validation.Dispose();
                }));
        }
    }

    /// <summary>Provides validation rule extension members for <paramref name="viewModel"/>.</summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="viewModel">The view model to attach validation rules to.</param>
    extension<TViewModel>(TViewModel viewModel)
        where TViewModel : class, IReactiveObject, IValidatableViewModel
    {
        /// <summary>Setup a validation rule for a specified ViewModel property with static error message.</summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="message"/> is empty.</exception>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp>(
            Expression<Func<TViewModel, TViewModelProp?>> viewModelProperty,
            Func<TViewModelProp?, bool> isPropertyValid,
            string message)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(isPropertyValid);

            ArgumentExceptionHelper.ThrowIfNullOrEmpty(message);

            // We need to associate the ViewModel property with
            // something that can be easily looked up and bound to.
            return viewModel.RegisterValidation(
                new BasePropertyValidation<TViewModel, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    isPropertyValid,
                    message));
        }

        /// <summary>Setup a validation rule for a specified ViewModel property with dynamic error message.</summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="isPropertyValid">Func to define if the viewModelProperty is valid or not.</param>
        /// <param name="message">Func to define the validation error message based on the viewModelProperty value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp>(
            Expression<Func<TViewModel, TViewModelProp?>> viewModelProperty,
            Func<TViewModelProp?, bool> isPropertyValid,
            Func<TViewModelProp?, string> message)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(isPropertyValid);

            ArgumentExceptionHelper.ThrowIfNull(message);

            return viewModel.RegisterValidation(
                new BasePropertyValidation<TViewModel, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    isPropertyValid,
                    message));
        }

        /// <summary>Setup a validation rule with a general observable indicating validity and a static error message.</summary>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/>, <paramref name="validationObservable"/>, or <paramref name="message"/> is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule(
            IObservable<bool> validationObservable,
            string message)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            ArgumentExceptionHelper.ThrowIfNull(message);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    viewModel,
                    validationObservable,
                    static validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity with a dynamic
        /// validation function and a dynamic context-aware error message.
        /// </summary>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TValue>(
            IObservable<TValue> validationObservable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            ArgumentExceptionHelper.ThrowIfNull(isValidFunc);

            ArgumentExceptionHelper.ThrowIfNull(messageFunc);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, TValue>(
                    viewModel,
                    validationObservable,
                    isValidFunc,
                    messageFunc));
        }

        /// <summary>Setup a validation rule with a general observable based on <see cref="IValidationState"/>.</summary>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> or <paramref name="validationObservable"/> is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule(
            IObservable<IValidationState> validationObservable)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    validationObservable));
        }

        /// <summary>Setup a validation rule with a general observable based on <see cref="IValidationState"/>.</summary>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> or <paramref name="validationObservable"/> is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TValue>(
            IObservable<TValue> validationObservable)
            where TValue : IValidationState
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool>(
                    validationObservable.Select(static s => s as IValidationState)));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity and a static error message
        /// for the given view model property.
        /// </summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="message">Validation error message.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp>(
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<bool> viewModelObservable,
            string message)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(viewModelObservable);

            ArgumentExceptionHelper.ThrowIfNull(message);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservable,
                    static validity => validity,
                    message));
        }

        /// <summary>
        /// Setup a validation rule with a general observable indicating validity with a dynamic
        /// validation function and a dynamic context-aware error message for the given view model property.
        /// </summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="viewModelObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="isValidFunc">Func to define if the value emitted by the observable is valid.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the observable value.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp, TValue>(
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<TValue> viewModelObservable,
            Func<TValue, bool> isValidFunc,
            Func<TValue, string> messageFunc)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(viewModelObservable);

            ArgumentExceptionHelper.ThrowIfNull(isValidFunc);

            ArgumentExceptionHelper.ThrowIfNull(messageFunc);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, TValue, TViewModelProp>(
                    viewModel,
                    viewModelProperty,
                    viewModelObservable,
                    isValidFunc,
                    messageFunc));
        }

        /// <summary>Setup a validation rule with a general observable based on <see cref="IValidationState"/>.</summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp>(
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<IValidationState> validationObservable)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModelProperty,
                    validationObservable));
        }

        /// <summary>Setup a validation rule with a general observable based on <see cref="IValidationState"/>.</summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <typeparam name="TValue">Validation observable type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property referenced in viewModelObservableProperty.</param>
        /// <param name="validationObservable">Observable to define if the viewModel is valid or not.</param>
        /// <returns>Returns a <see cref="ValidationHelper"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        /// <remarks>
        /// It should be noted that the observable should provide an initial value, otherwise that can result
        /// in an inconsistent performance.
        /// </remarks>
        [RequiresDynamicCode("WhenAnyValue uses expression trees which require dynamic code generation in AOT scenarios.")]
        [RequiresUnreferencedCode("WhenAnyValue may reference members that could be trimmed in AOT scenarios.")]
        public ValidationHelper ValidationRule<TViewModelProp, TValue>(
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            IObservable<TValue> validationObservable)
            where TValue : IValidationState
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            ArgumentExceptionHelper.ThrowIfNull(validationObservable);

            return viewModel.RegisterValidation(
                new ObservableValidation<TViewModel, bool, TViewModelProp>(
                    viewModelProperty,
                    validationObservable.Select(static v => v as IValidationState)));
        }

        /// <summary>Clears the validation rules associated with the specified property.</summary>
        /// <typeparam name="TViewModelProp">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">The property for which we are clearing the validation rules.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        public void ClearValidationRules<TViewModelProp>(
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty)
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            var propertyName = viewModelProperty.Body.GetPropertyPath();
            List<IValidationComponent> validationComponents = [];
            foreach (var validation in viewModel.ValidationContext.Validations.Items)
            {
                if (validation is IPropertyValidationComponent propertyValidation
                    && propertyValidation.ContainsPropertyName(propertyName))
                {
                    validationComponents.Add(propertyValidation);
                }
            }

            viewModel
                .ValidationContext
                .RemoveMany(validationComponents);
        }

        /// <summary>Removes all validation rules associated with a view model.</summary>
        /// <exception cref="ArgumentNullException">Thrown when any argument is null.</exception>
        public void ClearValidationRules()
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            viewModel.ValidationContext.RemoveMany(viewModel.ValidationContext.Validations.Items);
        }

        /// <summary>Gets an observable for the validity of the ViewModel.</summary>
        /// <returns>Returns true if the ValidationContext is valid, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewModel"/> is null.</exception>
        public IObservable<bool> IsValid()
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModel);

            return viewModel.ValidationContext.Valid;
        }
    }
}
