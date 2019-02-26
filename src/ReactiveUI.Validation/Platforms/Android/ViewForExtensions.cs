using System;
using System.Linq.Expressions;
using Android.Support.Design.Widget;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.DroidExtensions.Extensions
{
    public static class ViewForExtensions
    {
        /// <summary>
        /// Platform binding to the <see cref="TextInputLayout"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProp"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation
            <TView, TViewModel, TViewModelProp>(this TView view,
                                                TViewModel viewModel,
                                                Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
                                                TextInputLayout viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForProperty(view, viewModelProperty,
                (state, errorText) => viewProperty.Error = errorText, SingleLineFormatter.Default);
        }

        /// <summary>
        /// Platform binding to the <see cref="TextInputLayout"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation
            <TView, TViewModel>(this TView view,
                                TViewModel viewModel,
                                Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
                                TextInputLayout viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty,
                (state, errorText) =>
                {
                    viewProperty.Error = errorText;
                }, SingleLineFormatter.Default);
        }
    }
}
