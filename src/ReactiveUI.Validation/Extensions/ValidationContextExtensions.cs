// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DynamicData;

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Extensions;
#else
namespace ReactiveUI.Validation.Extensions;
#endif

/// <summary>Extensions methods for <see cref="ValidationContext"/>.</summary>
public static class ValidationContextExtensions
{
    /// <summary>
    /// Gets the seed value used by <c>CombineLatest().StartWith()</c> to ensure subscribers receive
    /// an initial valid state before any validation components have emitted.
    /// </summary>
    private static IValidationState[] InitialValidationStates { get; } = [ValidationState.Valid];

    /// <summary>Provides ObserveFor extension members for <paramref name="context"/>.</summary>
    /// <param name="context">The validation context to observe.</param>
    extension(IValidationContext context)
    {
        /// <summary>Resolves the <see cref="IValidationState"/> for a specified property in a reactive fashion, requiring the property to be unique.</summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> or <paramref name="viewModelProperty"/> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IObservable<IList<IValidationState>> ObserveFor<TViewModel, TViewModelProperty>(
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty) =>
            context.ObserveFor(viewModelProperty, true);

        /// <summary>Resolves the <see cref="IValidationState"/> for a specified property in a reactive fashion.</summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TViewModelProperty">ViewModel property type.</typeparam>
        /// <param name="viewModelProperty">ViewModel property.</param>
        /// <param name="strict">Indicates if the ViewModel property to find is unique.</param>
        /// <returns>Returns a collection of <see cref="BasePropertyValidation{TViewModel}"/> objects.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> or <paramref name="viewModelProperty"/> is null.</exception>
        public IObservable<IList<IValidationState>> ObserveFor<TViewModel, TViewModelProperty>(
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            bool strict)
        {
            ArgumentExceptionHelper.ThrowIfNull(context);

            ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);

            var propertyName = viewModelProperty.Body.GetPropertyPath();

            return context
                .Validations
                .Connect()
                .ToCollection()
                .Select(validations =>
                    System.Reactive.Linq.Observable.CombineLatest(SelectStatusChanges(validations, propertyName, strict))
                        .StartWith(InitialValidationStates))
                .SwitchTo();
        }
    }

    /// <summary>Selects the status change streams of the property validations that cover a property.</summary>
    /// <param name="validations">The validation components to search.</param>
    /// <param name="propertyName">The property name to look for.</param>
    /// <param name="strict">Indicates if the property to find is unique.</param>
    /// <returns>The status change streams of the matching property validations.</returns>
    private static List<IObservable<IValidationState>> SelectStatusChanges(
        IReadOnlyCollection<IValidationComponent> validations,
        string propertyName,
        bool strict)
    {
        List<IObservable<IValidationState>> statusChanges = [];
        foreach (var validation in validations)
        {
            if (validation is IPropertyValidationComponent propertyValidation
                && propertyValidation.ContainsPropertyName(propertyName, strict))
            {
                statusChanges.Add(propertyValidation.ValidationStatusChange);
            }
        }

        return statusChanges;
    }
}
