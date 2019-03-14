using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.ValidationBindings;
using Splat;

namespace ReactiveUI.Validation.Extensions
{
    /// <summary>
    /// Extensions methods associated to <see cref="IViewFor"/> instances.
    /// </summary>
    public static class ViewForExtensions
    {
        public static IDisposable BindValidationEx<TView, TViewModel, TViewModelProperty, TViewProperty>(this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationExtendedBinding.ForProperty(view, viewModelProperty, viewProperty);
        }

        /// <summary>
        /// Bind the specified ViewModel property validation to the View property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation<TView, TViewModel, TViewModelProperty, TViewProperty>(this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForProperty(view, viewModelProperty, viewProperty);
        }

        /// <summary>
        /// Bind the overall validation of a View model to a specified View property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(this TView view,
            TViewModel viewModel,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForViewModel<TView, TViewModel, TViewProperty>(view, viewProperty);
        }

        /// <summary>
        /// Bind a <see cref="ValidationHelper" /> from a ViewModel to a specified View property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation<TView, TViewModel, TViewProperty>(this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty, viewProperty);
        }

        public static IDisposable BindToDirect<TTarget, TValue>(IObservable<TValue> This,
            TTarget target,
            Expression viewExpression)
        {
            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());
            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
                return This.Subscribe(
                    x => setter(target, x, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.ErrorException($"{viewExpression} Binding received an Exception!", ex));

            var bindInfo = This.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new {val, host});

            return bindInfo
                .Where(x => x.host != null)
                .Subscribe(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => LogHost.Default.ErrorException($"{viewExpression} Binding received an Exception!", ex));
        }
    }
}