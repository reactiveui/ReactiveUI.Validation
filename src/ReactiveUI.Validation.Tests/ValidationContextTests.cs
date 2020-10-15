// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for <see cref="ValidationContext"/>.
    /// </summary>
    public class ValidationContextTests
    {
        /// <summary>
        /// Verifies that a <see cref="ValidationContext"/> without validations is valid.
        /// </summary>
        [Fact]
        public void EmptyValidationContextIsValid()
        {
            var vc = new ValidationContext(ImmediateScheduler.Instance);

            Assert.True(vc.IsValid);
            Assert.Equal(0, vc.Text.Count);
        }

        /// <summary>
        /// Verifies that validations can be added in the <see cref="ValidationContext"/>.
        /// </summary>
        [Fact]
        public void CanAddValidationComponentsTest()
        {
            var vc = new ValidationContext(ImmediateScheduler.Instance);

            var invalidName = string.Empty;

            var vm = new TestViewModel { Name = "valid" };

            var v1 = new BasePropertyValidation<TestViewModel, string>(
                vm,
                v => v.Name,
                s => !string.IsNullOrEmpty(s),
                msg => $"{msg} isn't valid");

            vc.Add(v1);

            Assert.True(vc.IsValid);

            vm.Name = invalidName;

            Assert.False(v1.IsValid);
            Assert.False(vc.IsValid);

            Assert.Equal(1, vc.Text.Count);
        }

        /// <summary>
        /// Verifies that two validations properties are correctly applied in the <see cref="ValidationContext"/>.
        /// </summary>
        [Fact]
        public void TwoValidationComponentsCorrectlyResultInContextTest()
        {
            const string validName = "valid";
            var invalidName = string.Empty;

            var vc = new ValidationContext(ImmediateScheduler.Instance);

            var vm = new TestViewModel { Name = validName, Name2 = validName };

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(
                vm,
                v => v.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            var secondValidation = new BasePropertyValidation<TestViewModel, string>(
                vm,
                v => v.Name2,
                s => !string.IsNullOrEmpty(s),
                s => $"Name 2 {s} isn't valid");

            vc.Add(firstValidation);
            vc.Add(secondValidation);

            Assert.True(vc.IsValid);
            Assert.Equal(0, vc.Text.Count);

            vm.Name = invalidName;
            Assert.False(vc.IsValid);
            Assert.Equal(1, vc.Text.Count);
            Assert.Equal("Name " + invalidName + " isn't valid", vc.Text[0]);

            vm.Name2 = invalidName;
            Assert.False(vc.IsValid);
            Assert.Equal(2, vc.Text.Count);
            Assert.Equal("Name " + invalidName + " isn't valid", vc.Text[0]);
            Assert.Equal("Name 2 " + invalidName + " isn't valid", vc.Text[1]);

            vm.Name = validName;
            vm.Name2 = validName;

            Assert.True(vc.IsValid);
            Assert.Equal(0, vc.Text.Count);
        }

        /// <summary>
        /// Verifies that this.IsValid() extension method observes a
        /// <see cref="ValidationContext"/> and emits new values.
        /// </summary>
        [Fact]
        public void IsValidShouldNotifyOfValidityChange()
        {
            var viewModel = new TestViewModel { Name = string.Empty };
            var nameValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                "Name should not be empty.");
            viewModel.ValidationContext.Add(nameValidation);

            var latestValidity = false;
            viewModel.IsValid().Subscribe(isValid => latestValidity = isValid);
            Assert.False(latestValidity);

            viewModel.Name = "Jonathan";
            Assert.True(latestValidity);

            viewModel.Name = string.Empty;
            Assert.False(latestValidity);
        }

        /// <summary>
        /// Ensures that the ClearValidationRules extension method works.
        /// </summary>
        [Fact]
        public void ShouldClearAttachedValidationRules()
        {
            var viewModel = new TestViewModel { Name = string.Empty };
            var nameValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                "Name should not be empty.");

            var name2Validation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name2,
                s => !string.IsNullOrEmpty(s),
                "Name2 should not be empty.");

            viewModel.ValidationContext.Add(nameValidation);
            viewModel.ValidationContext.Add(name2Validation);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.NotEmpty(viewModel.ValidationContext.Text);

            viewModel.ClearValidationRules();
            Assert.Equal(0, viewModel.ValidationContext.Validations.Count);
            Assert.True(viewModel.ValidationContext.IsValid);
            Assert.Empty(viewModel.ValidationContext.Text);
        }

        /// <summary>
        /// Ensures that the ClearValidationRules extension method accepting an expression works.
        /// </summary>
        [Fact]
        public void ShouldClearAttachedValidationRulesForTheGivenProperty()
        {
            var viewModel = new TestViewModel { Name = string.Empty };
            var nameValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                "Name should not be empty.");

            const string name2ErrorMessage = "Name2 should not be empty.";
            var name2Validation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name2,
                s => !string.IsNullOrEmpty(s),
                name2ErrorMessage);

            viewModel.ValidationContext.Add(nameValidation);
            viewModel.ValidationContext.Add(name2Validation);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.NotEmpty(viewModel.ValidationContext.Text);

            viewModel.ClearValidationRules(x => x.Name);
            Assert.Equal(1, viewModel.ValidationContext.Validations.Count);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.NotEmpty(viewModel.ValidationContext.Text);
            Assert.Equal(name2ErrorMessage, viewModel.ValidationContext.Text.ToSingleLine());
        }
    }
}
