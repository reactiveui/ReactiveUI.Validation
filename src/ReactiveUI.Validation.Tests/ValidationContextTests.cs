using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    public class ValidationContextTests
    {
        [Fact]
        public void EmptyValidationContextIsValid()
        {
            var vc = new ValidationContext();

            Assert.True(vc.IsValid);
            Assert.Equal(0, vc.Text.Count);
        }

        [Fact]
        public void CanAddValidationComponentsTest()
        {
            var vc = new ValidationContext();

            var invalidName = string.Empty;

            var vm = new TestViewModel {Name = "valid"};

            var v1 = new BasePropertyValidation<TestViewModel, string>(vm,
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

        [Fact]
        public void TwoValidationComponentsCorrectlyResultInContextTest()
        {
            const string validName = "valid";
            var invalidName = string.Empty;

            var vc = new ValidationContext();

            var vm = new TestViewModel {Name = validName, Name2 = validName};

            var v1 = new BasePropertyValidation<TestViewModel, string>(vm,
                v => v.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            var v2 = new BasePropertyValidation<TestViewModel, string>(vm,
                v => v.Name2,
                s => !string.IsNullOrEmpty(s),
                s => $"Name 2 {s} isn't valid");

            vc.Add(v1);
            vc.Add(v2);

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

        [Fact]
        public void TwoValidationPropertiesInSamePropertyThrowsExceptionTest()
        {
            const string validName = "valid";
            const int minimumLength = 5;

            var viewModel = new TestViewModel {Name = validName};
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var secondValidation = new BasePropertyValidation<TestViewModel, string>(viewModel,
                vm => vm.Name,
                s => s.Length > minimumLength,
                s => minimumLengthErrorMessage);

            // Add validations
            viewModel.ValidationContext.Add(firstValidation);
            viewModel.ValidationContext.Add(secondValidation);

            // View bindings
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);

            // TODO: add Assert.Throws to custom exception wrapping this call
            // View validations bindings
            view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameLabelError);

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            Assert.Equal(minimumLengthErrorMessage, view.NameLabelError);
        }

        [Fact]
        public void TwoValidationPropertiesInSamePropertyResultsTest()
        {
            const string validName = "valid";
            const int minimumLength = 5;

            var viewModel = new TestViewModel {Name = validName};
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                s => $"Name {s} isn't valid");

            var minimumLengthErrorMessage = $"Minimum length is {minimumLength}";
            var secondValidation = new BasePropertyValidation<TestViewModel, string>(viewModel,
                vm => vm.Name,
                s => s.Length > minimumLength,
                s => minimumLengthErrorMessage);

            // Add validations
            viewModel.ValidationContext.Add(firstValidation);
            viewModel.ValidationContext.Add(secondValidation);

            // View bindings
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);

            // View validations bindings
            view.BindValidation(view.ViewModel, vm => vm.Name, v => v.NameLabelError);

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            Assert.Equal(minimumLengthErrorMessage, view.NameLabelError);
        }
    }
}