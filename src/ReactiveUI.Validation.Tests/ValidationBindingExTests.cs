using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    public class ValidationBindingExTests
    {
        [Fact]
        public void TwoValidationPropertiesInSamePropertyResultsTest()
        {
            const int minimumLength = 5;

            var viewModel = new TestViewModel {Name = "some"};
            var view = new TestView(viewModel);

            var firstValidation = new BasePropertyValidation<TestViewModel, string>(viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                "Name is required.");

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
            view.BindValidationEx(view.ViewModel, vm => vm.Name, v => v.NameLabelError);

            viewModel.Name = "som";

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);

            // Checks if second validation error message is shown
            Assert.Equal(minimumLengthErrorMessage, view.NameLabelError);
        }
    }
}