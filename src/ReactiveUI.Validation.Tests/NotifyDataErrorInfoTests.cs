using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for INotifyDataErrorInfo support.
    /// </summary>
    public class NotifyDataErrorInfoTests
    {
        private const string NameShouldNotBeEmptyMessage = "Name shouldn't be empty.";

        /// <summary>
        /// Verifies that the ErrorsChanged event fires on ViewModel initialization.
        /// </summary>
        [Fact]
        public void ShouldMarkPropertiesAsInvalidOnInit()
        {
            var viewModel = new IndeiTestViewModel();
            var view = new IndeiTestView(viewModel);

            var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                NameShouldNotBeEmptyMessage);

            viewModel.ValidationContext.Add(firstValidation);
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidationEx(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

            // Verify validation context behavior.
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Single(viewModel.ValidationContext.Validations);
            Assert.Equal(NameShouldNotBeEmptyMessage, view.NameErrorLabel);

            // Verify INotifyDataErrorInfo behavior.
            Assert.True(viewModel.HasErrors);
            Assert.Equal(NameShouldNotBeEmptyMessage, viewModel.GetErrors("Name").Cast<string>().First());
        }

        /// <summary>
        /// Verifies that the view model listens to the INotifyPropertyChanged event
        /// and sends INotifyDataErrorInfo notifications.
        /// </summary>
        [Fact]
        public void ShouldSynchronizeNotifyDataErrorInfoWithValidationContext()
        {
            var viewModel = new IndeiTestViewModel();
            var view = new IndeiTestView(viewModel);

            var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                NameShouldNotBeEmptyMessage);

            viewModel.ValidationContext.Add(firstValidation);
            view.Bind(view.ViewModel, vm => vm.Name, v => v.NameLabel);
            view.BindValidationEx(view.ViewModel, vm => vm.Name, v => v.NameErrorLabel);

            // Verify the initial state.
            Assert.True(viewModel.HasErrors);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Single(viewModel.ValidationContext.Validations);
            Assert.Equal(NameShouldNotBeEmptyMessage, viewModel.GetErrors("Name").Cast<string>().First());
            Assert.Equal(NameShouldNotBeEmptyMessage, view.NameErrorLabel);

            // Send INotifyPropertyChanged.
            viewModel.Name = "JoJo";

            // Verify the changed state.
            Assert.False(viewModel.HasErrors);
            Assert.True(viewModel.ValidationContext.IsValid);
            Assert.Empty(viewModel.GetErrors("Name").Cast<string>());
            Assert.Empty(view.NameErrorLabel);

            // Send INotifyPropertyChanged.
            viewModel.Name = string.Empty;

            // Verify the changed state.
            Assert.True(viewModel.HasErrors);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Single(viewModel.ValidationContext.Validations);
            Assert.Equal(NameShouldNotBeEmptyMessage, viewModel.GetErrors("Name").Cast<string>().First());
            Assert.Equal(NameShouldNotBeEmptyMessage, view.NameErrorLabel);
        }

        /// <summary>
        /// The ErrorsChanged event should fire when properties change.
        /// </summary>
        [Fact]
        public void ShouldFireErrorsChangedEventWhenValidationStateChanges()
        {
            var viewModel = new IndeiTestViewModel();

            DataErrorsChangedEventArgs arguments = null;
            viewModel.ErrorsChanged += (sender, args) => arguments = args;

            var firstValidation = new BasePropertyValidation<IndeiTestViewModel, string>(
                viewModel,
                vm => vm.Name,
                s => !string.IsNullOrEmpty(s),
                NameShouldNotBeEmptyMessage);

            viewModel.ValidationContext.Add(firstValidation);

            Assert.True(viewModel.HasErrors);
            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Single(viewModel.ValidationContext.Validations);
            Assert.Single(viewModel.GetErrors("Name").Cast<string>());

            viewModel.Name = "JoJo";

            Assert.False(viewModel.HasErrors);
            Assert.Empty(viewModel.GetErrors("Name").Cast<string>());
            Assert.NotNull(arguments);
            Assert.Equal(string.Empty, arguments.PropertyName);
        }

        /// <summary>
        /// Using ModelObservableValidation with NotifyDataErrorInfo should return errors when associated property changes.
        /// </summary>
        [Fact]
        public void ShouldDeliverErrorsWhenModelObservableValidationTriggers()
        {
            var viewModel = new IndeiTestViewModel();

            string namesShouldMatchMessage = "names should match.";
            var validation = new ModelObservableValidation<IndeiTestViewModel, string>(
                viewModel,
                vm => vm.OtherName,
                vm => vm.WhenAnyValue(
                        m => m.Name,
                        m => m.OtherName,
                        (n, on) => new { n, on })
                    .Select(bothNames => bothNames.n == bothNames.on),
                (_, isValid) => isValid ? string.Empty : namesShouldMatchMessage);

            viewModel.ValidationContext.Add(validation);

            Assert.False(viewModel.HasErrors);
            Assert.True(viewModel.ValidationContext.IsValid);
            Assert.Single(viewModel.ValidationContext.Validations);
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>());
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>());

            viewModel.Name = "JoJo";
            viewModel.OtherName = "NoNo";

            Assert.True(viewModel.HasErrors);
            Assert.Empty(viewModel.GetErrors(nameof(viewModel.Name)).Cast<string>());
            Assert.Single(viewModel.GetErrors(nameof(viewModel.OtherName)).Cast<string>());
            Assert.Single(validation.Text);
            Assert.Equal(namesShouldMatchMessage, validation.Text.Single());
        }

        /// <summary>
        /// Verifies that validation rules of the same property do not duplicate.
        /// Earlier they sometimes could, due to the .Connect() method misuse.
        /// </summary>
        [Fact]
        public void ValidationRulesOfTheSamePropertyShouldNotDuplicate()
        {
            var viewModel = new IndeiTestViewModel();
            viewModel.ValidationRule(
                m => m.Name,
                m => m != null,
                "Name shouldn't be null.");

            viewModel.ValidationRule(
                m => m.Name,
                m => !string.IsNullOrWhiteSpace(m),
                "Name shouldn't be white space.");

            Assert.False(viewModel.ValidationContext.IsValid);
            Assert.Equal(2, viewModel.ValidationContext.Validations.Count);
        }
    }
}
