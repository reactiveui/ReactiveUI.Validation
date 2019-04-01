using System.Reactive.Concurrency;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Exceptions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using ReactiveUI.Validation.ValidationBindings;
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
        /// Verifies that a View property cannot have more than one validation and throws a <see cref="MultipleValidationNotSupportedException"/>.
        /// </summary>
        [Fact]
        public void TwoValidationPropertiesInSamePropertyThrowsExceptionTest()
        {
            const string validName = "valid";
            const int minimumLength = 5;

            var viewModel = new TestViewModel { Name = validName };
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            var secondValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => s.Length > minimumLength,
                _ => $"Minimum length is {minimumLength}");

            // Add validations
            viewModel.ValidationContext.Add(firstValidation);
            viewModel.ValidationContext.Add(secondValidation);

            // View bindings
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);

            // View validations bindings
            var ex = Assert.Throws<MultipleValidationNotSupportedException>(() =>
            {
                return view.BindValidation(
                    view.ViewModel,
                    vm => vm.Name,
                    v => v.NameErrorLabel);
            });

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            var expectedError =
                $"Property {nameof(viewModel.Name)} has more than one validation rule associated. Consider using {nameof(ValidationBindingEx)} methods.";
            Assert.Equal(expectedError, ex.Message);
        }

        /// <summary>
        /// Verifies that validations registered with different lambda names are retrieved successfully.
        /// </summary>
        [Fact]
        public void RegisterValidationsWithDifferentLambdaNameWorksTest()
        {
            const string validName = "valid";
            const int minimumLength = 5;

            var viewModel = new TestViewModel { Name = validName };
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(
                viewModel,
                viewModelProperty => viewModelProperty.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            // Add validations
            viewModel.ValidationContext.Add(firstValidation);

            // View bindings
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);

            // This was throwing exception due to naming problems with lambdas expressions
            view.BindValidation(
                view.ViewModel,
                vm => vm.Name,
                v => v.NameErrorLabel);

            Assert.True(viewModel.ValidationContext.IsValid);
            Assert.Equal(1, viewModel.ValidationContext.Validations.Count);
        }
    }
}