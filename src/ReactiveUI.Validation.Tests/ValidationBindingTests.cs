// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Contains tests for validation binding extensions.
    /// </summary>
    public class ValidationBindingTests
    {
        /// <summary>
        /// Verifies that two validations properties are correctly applied in a View property.
        /// </summary>
        [Fact]
        public void ShouldSupportBindingTwoValidationsForOneProperty()
        {
            const int minimumLength = 5;
            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var view = new TestView(new TestViewModel { Name = "some" });

            view.ViewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "Name is required.");

            view.ViewModel.ValidationRule(
                vm => vm.Name,
                s => s.Length > minimumLength,
                s => minimumLengthErrorMessage);

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

            view.ViewModel.Name = "som";

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Equal(2, view.ViewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            Assert.Equal(minimumLengthErrorMessage, view.NameErrorLabel);
        }

        /// <summary>
        /// Verifies that two validations properties are correctly applied
        /// in a View property given by a complex expression.
        /// </summary>
        [Fact]
        public void ShouldSupportBindingTwoValidationsForOnePropertyToChainedViewProperties()
        {
            const int minimumLength = 5;
            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var view = new TestView(new TestViewModel { Name = "some" });

            view.ViewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "Name is required.");

            view.ViewModel.ValidationRule(
                vm => vm.Name,
                s => s.Length > minimumLength,
                minimumLengthErrorMessage);

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorContainer.Text);

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Equal(2, view.ViewModel.ValidationContext.Validations.Count);
            Assert.Equal(minimumLengthErrorMessage, view.NameErrorContainer.Text);
        }

        /// <summary>
        /// Verifies that validations registered with different lambda names are retrieved successfully.
        /// </summary>
        [Fact]
        public void RegisterValidationsWithDifferentLambdaNameWorksTest()
        {
            const string validName = "valid";
            var view = new TestView(new TestViewModel { Name = validName });

            view.ViewModel.ValidationRule(
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

            Assert.True(view.ViewModel.ValidationContext.IsValid);
            Assert.Single(view.ViewModel.ValidationContext.Validations);
        }

        /// <summary>
        /// Verifies that validation error messages get concatenated using white space.
        /// </summary>
        [Fact]
        public void ValidationMessagesDefaultConcatenationTest()
        {
            var view = new TestView(new TestViewModel { Name = string.Empty });

            view.ViewModel.ValidationRule(
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                "Name should not be empty.");

            view.ViewModel.ValidationRule(
                viewModelProperty => viewModelProperty.Name2,
                s => !string.IsNullOrEmpty(s),
                "Name2 should not be empty.");

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
            view.BindValidation(view.ViewModel, v => v.NameErrorLabel);

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Equal(2, view.ViewModel.ValidationContext.Validations.Count);
            Assert.NotEmpty(view.NameErrorLabel);
            Assert.Equal("Name should not be empty. Name2 should not be empty.", view.NameErrorLabel);
        }

        /// <summary>
        /// Property validations backed by ModelObservableValidationBase should
        /// be bound to view as well as base property validations are.
        /// </summary>
        [Fact]
        public void ComplexValidationRulesShouldBeBoundToView()
        {
            const string errorMessage = "Both inputs should be the same";
            var view = new TestView(new TestViewModel
            {
                Name = "Josuke Hikashikata",
                Name2 = "Jotaro Kujo"
            });

            view.ViewModel.ValidationRule(
                m => m.Name,
                m => m.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
                (vm, isValid) => isValid ? string.Empty : errorMessage);

            view.BindValidation(view.ViewModel, x => x.Name, x => x.NameErrorLabel);

            Assert.NotEmpty(view.NameErrorLabel);
            Assert.Equal(errorMessage, view.NameErrorLabel);
        }

        /// <summary>
        /// Using 2 validation rules ending with the same property name should not
        /// result in both properties having all the errors of both properties.
        /// </summary>
        [Fact]
        public void ErrorsWithTheSameLastPropertyShouldNotShareErrors()
        {
            var model = new SourceDestinationViewModel();
            var view = new SourceDestinationView(model);

            model.ValidationRule(
                viewModel => viewModel.Source.Name,
                name => !string.IsNullOrWhiteSpace(name),
                "Source text");

            model.ValidationRule(
                viewModel => viewModel.Destination.Name,
                name => !string.IsNullOrWhiteSpace(name),
                "Destination text");

            view.BindValidation(view.ViewModel, x => x.Source.Name, x => x.SourceError);
            view.BindValidation(view.ViewModel, x => x.Destination.Name, x => x.DestinationError);

            Assert.NotNull(view.SourceError);
            Assert.Equal("Source text", view.SourceError);
            Assert.Equal("Destination text", view.DestinationError);
        }

        /// <summary>
        /// Verifies that we still support binding to <see cref="ValidationHelper" /> properties.
        /// </summary>
        [Fact]
        public void ShouldSupportBindingValidationHelperProperties()
        {
            const string nameErrorMessage = "Name should not be empty.";
            var view = new TestView(new TestViewModel { Name = string.Empty });

            view.ViewModel.NameRule = view
                .ViewModel
                .ValidationRule(
                    viewModelProperty => viewModelProperty.Name,
                    s => !string.IsNullOrEmpty(s),
                    nameErrorMessage);

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidation(view.ViewModel, vm => vm.NameRule, v => v.NameErrorLabel);

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Single(view.ViewModel.ValidationContext.Validations);
            Assert.Equal(nameErrorMessage, view.NameErrorLabel);

            view.ViewModel.Name = "Jonathan";

            Assert.True(view.ViewModel.ValidationContext.IsValid);
            Assert.Empty(view.NameErrorLabel);

            view.ViewModel.Name = string.Empty;

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Equal(nameErrorMessage, view.NameErrorLabel);
        }

        /// <summary>
        /// Verifies that bindings support model observable validations.
        /// </summary>
        [Fact]
        public void ShouldSupportBindingModelObservableValidationHelperProperties()
        {
            const string namesShouldMatchMessage = "Names should match.";
            var view = new TestView(new TestViewModel
            {
                Name = "Bingo",
                Name2 = "Bongo"
            });

            view.ViewModel.NameRule = view
                .ViewModel
                .ValidationRule(
                    vm => vm.Name2,
                    vm => vm.WhenAnyValue(x => x.Name, x => x.Name2, (name, name2) => name == name2),
                    (vm, valid) => valid ? string.Empty : namesShouldMatchMessage);

            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.Bind(view.ViewModel, vm => vm.Name2, v => v.Name2Label);
            view.BindValidation(view.ViewModel, vm => vm.NameRule, v => v.NameErrorLabel);

            Assert.False(view.ViewModel.ValidationContext.IsValid);
            Assert.Single(view.ViewModel.ValidationContext.Validations);
            Assert.Equal(namesShouldMatchMessage, view.NameErrorLabel);

            view.ViewModel.Name = "Bongo";
            view.ViewModel.Name2 = "Bongo";

            Assert.True(view.ViewModel.ValidationContext.IsValid);
            Assert.Single(view.ViewModel.ValidationContext.Validations);
            Assert.Empty(view.NameErrorLabel);
        }

        /// <summary>
        /// Verifies that the IsValid and Message properties of a
        /// <see cref="ValidationHelper" /> produce change notifications.
        /// </summary>
        [Fact]
        public void ShouldUpdateBindableValidationHelperIsValidProperty()
        {
            const string nameErrorMessage = "Name should not be empty.";
            var view = new TestView(new TestViewModel { Name = string.Empty });

            view.ViewModel.NameRule = view
                .ViewModel
                .ValidationRule(
                    viewModelProperty => viewModelProperty.Name,
                    s => !string.IsNullOrEmpty(s),
                    nameErrorMessage);

            view.OneWayBind(view.ViewModel, vm => vm.NameRule.IsValid, v => v.IsNameValid);
            view.OneWayBind(view.ViewModel, vm => vm.NameRule.Message, v => v.NameErrorLabel, s => s.ToSingleLine());

            Assert.False(view.IsNameValid);
            Assert.Equal(nameErrorMessage, view.NameErrorLabel);

            view.ViewModel.Name = "Bingo";

            Assert.True(view.IsNameValid);
            Assert.Empty(view.NameErrorLabel);
        }
    }
}