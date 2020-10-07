// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for BindValidationEx methods from the <see cref="ReactiveUI.Validation.ValidationBindings.ValidationBindingEx"/> Mixins.
    /// </summary>
    public class ValidationBindingExTests
    {
        /// <summary>
        /// Verifies that two validations properties are correctly applied in a View property.
        /// </summary>
        [Fact]
        public void TwoValidationPropertiesInSamePropertyResultsTest()
        {
            const int minimumLength = 5;

            var viewModel = new TestViewModel { Name = "some" };
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "Name is required.");

            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var secondValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => s.Length > minimumLength,
                s => minimumLengthErrorMessage);

            // Add validations
            viewModel.ValidationContext.Add(firstValidation);
            viewModel.ValidationContext.Add(secondValidation);

            // View bindings
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);

            // View validations bindings
            view.BindValidationEx(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

            viewModel.Name = "som";

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            Assert.Equal(minimumLengthErrorMessage, view.NameErrorLabel);
        }

        /// <summary>
        /// Verifies that two validations properties are correctly applied
        /// in a View property given by a complex expression.
        /// </summary>
        [Fact]
        public void TwoValidationPropertiesInSamePropertyShouldSupportBindingToNestedControls()
        {
            const int minimumLength = 5;
            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var viewModel = new TestViewModel { Name = "some" };
            var view = new TestView(viewModel);

            // Define the validations.
            viewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "Name is required.");

            viewModel.ValidationRule(
                vm => vm.Name,
                s => s.Length > minimumLength,
                minimumLengthErrorMessage);

            // Define view bindings.
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidationEx(view.ViewModel, vm => vm.Name, v => v.NameErrorContainer.Text);

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);
            Assert.Equal(minimumLengthErrorMessage, view.NameErrorContainer.Text);
        }
    }
}
