// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ReactiveUI.Validation.Collections;
using ReactiveUI.Validation.Components.Abstractions;

namespace ReactiveUI.Validation.Components
{
    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// Validates a single property. Though in the passed validityObservable more properties can be referenced.
    /// We probably need a more 'complex' one, where the params of the validation block are
    /// passed through?
    /// Also, what about access to the view model to output the error message?.
    /// </remarks>
    public class ModelObservableValidation<TViewModel, TViewModelProp> : ModelObservableValidationBase<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, viewModelProperty, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel, TProperty1}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="viewModelProperty">ViewModel property referenced in validityObservable.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, ValidationText> messageFunc)
            : base(viewModel, validityObservable, messageFunc)
        {
            // record this property name
            AddProperty(viewModelProperty);
        }
    }

    /// <inheritdoc cref="ReactiveObject" />
    /// <inheritdoc cref="IValidationComponent" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    /// More generic observable for determination of validity.
    /// </summary>
    /// <remarks>
    /// for backwards compatibility, validated properties are not explicitly defined, so we don't really know what's inside the validityObservable.
    /// </remarks>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same class just different generic parameters.")]
    public class ModelObservableValidation<TViewModel> : ModelObservableValidationBase<TViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, string> messageFunc)
            : this(viewModel, validityObservable, (vm, state) => new ValidationText(messageFunc(vm, state)))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelObservableValidation{TViewModel}"/> class.
        /// </summary>
        /// <param name="viewModel">ViewModel instance.</param>
        /// <param name="validityObservable">Observable to define if the viewModel is valid or not.</param>
        /// <param name="messageFunc">Func to define the validation error message based on the viewModel and validityObservable values.</param>
        public ModelObservableValidation(
            TViewModel viewModel,
            Func<TViewModel, IObservable<bool>> validityObservable,
            Func<TViewModel, bool, ValidationText> messageFunc)
            : base(viewModel, validityObservable, messageFunc)
        {
        }
    }
}
