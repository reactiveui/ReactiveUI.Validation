using System;
using System.Linq.Expressions;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Extensions
{
    public static class SupportsValidationExtensions
    {
        /// <summary>
        /// Validation rule for a property of a view model
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProp"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewPropertyValid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModelProp, bool> viewPropertyValid,
            string message)
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            // we need to associate the view model property 
            // with something that can be easily looked up and bound to

            var propValidation = new BasePropertyValidation<TViewModel, TViewModelProp>(viewModel, viewModelProperty,
                viewPropertyValid, message);

            viewModel.ValidationContext.Add(propValidation);

            var validationHelper = new ValidationHelper(propValidation);

            return validationHelper;
        }

        /// <summary>
        ///     Setup a validation rule for a specified view model property
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProp"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewPropertyValid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValidationHelper ValidationRule<TViewModel, TViewModelProp>(this TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModelProp, bool> viewPropertyValid,
            Func<TViewModelProp, string> message)
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            // we need to associate the view model property 
            // with something that can be easily looked up and bound to

            var propValidation = new BasePropertyValidation<TViewModel, TViewModelProp>(viewModel, viewModelProperty,
                viewPropertyValid, message);

            viewModel.ValidationContext.Add(propValidation);

            return new ValidationHelper(propValidation);
        }

        /// <summary>
        ///     Setup a rule with a general observable indicating validity.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <param name="validationObservableFunc"></param>
        /// <param name="messageFunc"></param>
        /// <returns></returns>
        /// <remarks>
        ///     It should be noted that the observable should provide an initial value, otherwise that can result in an
        ///     inconsistent performance.
        /// </remarks>
        public static ValidationHelper ValidationRule<TViewModel>(this TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validationObservableFunc,
            Func<TViewModel, bool, string> messageFunc)
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            var validation =
                new ModelObservableValidation<TViewModel>(viewModel, validationObservableFunc, messageFunc);

            viewModel.ValidationContext.Add(validation);

            return new ValidationHelper(validation);
        }

        /// <summary>
        ///     Get an observable for the validity of the view model.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public static IObservable<bool> IsValid<TViewModel>(this TViewModel viewModel)
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            return viewModel.ValidationContext.Valid;
        }
    }
}