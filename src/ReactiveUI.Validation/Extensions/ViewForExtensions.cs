using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;

namespace ReactiveUI.Validation.Extensions
{
    public static class ViewForExtensions
    {
        /// <summary>
        ///     Bind the specified view property validation to the view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty1"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty1"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation
            <TView, TViewModel, TViewModelProperty1, TViewProperty>(this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TViewModelProperty1>>
                    viewModelProperty1,
                Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForProperty(view, viewModelProperty1, viewProperty);
        }

        /// <summary>
        ///     Bind the overall validation of a view model to a specified view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation
            <TView, TViewModel, TViewProperty>(this TView view,
                TViewModel viewModel,
                Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForViewModel<TView, TViewModel, TViewProperty>(view, viewProperty);
        }


        /// <summary>
        ///     Bind a <see cref="ValidationHelper" /> from a view model to a specified view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation
            <TView, TViewModel, TViewProperty>(this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
                Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty, viewProperty);
        }


        public static IDisposable BindToDirect
            <TTarget, TValue>(IObservable<TValue> This, TTarget target, Expression viewExpression)
        {
            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());
            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
                return This.Subscribe(
                    x => setter(target, x, viewExpression.GetArgumentsArray()),
                    ex =>
                    {
                        //this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    });

            var bindInfo = This.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new {val, host});

            return bindInfo
                .Where(x => x.host != null)
                .Subscribe(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex =>
                    {
                        //this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    });
        }
    }
}