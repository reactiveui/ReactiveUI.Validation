using System.ComponentModel;
using System.Linq;
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
    }
}
