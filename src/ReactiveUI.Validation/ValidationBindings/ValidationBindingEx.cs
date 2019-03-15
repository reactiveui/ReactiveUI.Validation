using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Formatters;
using ReactiveUI.Validation.Formatters.Abstractions;
using ReactiveUI.Validation.ValidationBindings.Abstractions;
using Splat;

namespace ReactiveUI.Validation.ValidationBindings
{
    /// <summary>
    /// An extended validation binding which supports multiple validations.
    /// </summary>
    public class ValidationBindingEx : IValidationBinding
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Create an instance with a specified observable for validation changes.
        /// </summary>
        /// <param name="validationObservable"></param>
        public ValidationBindingEx(IObservable<Unit> validationObservable)
        {
            _disposables.Add(validationObservable.Subscribe());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Create a binding between a view model property and a view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <param name="formatter"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty, TViewProperty>(TView view,
            Expression<Func<TViewModel, TViewModelProperty>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string> formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null) formatter = SingleLineFormatter.Default;

            var vcObs = view.WhenAnyValue(v => v.ViewModel)
                .Where(vm => vm != null)
                .Select(
                    viewModel =>
                    {
                        var validations = viewModel.ValidationContext.ResolveForMultiple(viewModelProperty, strict);
                        return validations.Select(x => x.ValidationStatusChange)
                            .CombineLatest();
                    })
                .Switch()
                .Select(states => states.Select(state => formatter.Format(state.Text)).ToList());

            var updateObs = BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Create a binding to a view property.
        /// </summary>
        /// <param name="valueChange"></param>
        /// <param name="target"></param>
        /// <param name="viewProperty"></param>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public static IObservable<TValue> BindToView<TView, TViewProperty, TTarget, TValue>(
            IObservable<TValue> valueChange,
            TTarget target,
            Expression<Func<TView, TViewProperty>> viewProperty)
            where TValue : List<string>
        {
            var viewExpression = Reflection.Rewrite(viewProperty.Body);

            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
                return valueChange
                    .Do(
                        x =>
                        {
                            setter(target, x.First(msg => !string.IsNullOrEmpty(msg)),
                                viewExpression.GetArgumentsArray());
                        },
                        ex => LogHost.Default.ErrorException($"{viewExpression} Binding received an Exception!", ex));

            var bindInfo = valueChange.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new {val, host});

            return bindInfo
                .Where(x => x.host != null)
                .Do(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => { LogHost.Default.ErrorException($"{viewExpression} Binding received an Exception!", ex); })
                .Select(v => v.val);
        }
    }
}