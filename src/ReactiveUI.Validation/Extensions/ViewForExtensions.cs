using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Exceptions;
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
        /// <summary>
        /// Binds the specified ViewModel property validation to the View property.
        /// </summary>
        /// <remarks>Supports multiple validations for the same property.</remarks>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <returns></returns>
        public static IDisposable BindValidationEx<TView, TViewModel, TViewModelProperty, TViewProperty>(
            this TView view,
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBindingEx.ForProperty(view, viewModelProperty, viewProperty);
        }

        /// <summary>
        /// Binds the specified ViewModel property validation to the View property.
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
        /// <exception cref="MultipleValidationNotSupportedException"></exception>
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
        /// Binds the overall validation of a ViewModel to a specified View property.
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
        /// Binds a <see cref="ValidationHelper" /> from a ViewModel to a specified View property.
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

        /// <summary>
        /// Creates a binding to a View property.
        /// </summary>
        /// <param name="This"></param>
        /// <param name="target"></param>
        /// <param name="viewExpression"></param>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
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