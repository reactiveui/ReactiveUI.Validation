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

            var updateObs = ValidationBinding.BindToView(vcObs, view, viewProperty)
                .Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }
    }
}