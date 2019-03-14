using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.States;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    public class PropertyValidationTests
    {
        /// <summary>
        /// Default state is true.
        /// </summary>
        [Fact]
        public void ValidModelDefaultState()
        {
            var model = CreateDefaultValidModel();

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                vm => vm.Name,
                n => !string.IsNullOrEmpty(n),
                "broken");

            Assert.True(validation.IsValid);
            Assert.True(string.IsNullOrEmpty(validation.Text.ToSingleLine()));
        }

        [Fact]
        public void StateTransitionsWhenValidityChangesTest()
        {
            const string testValue = "test";

            var model = new TestViewModel();

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                vm => vm.Name,
                n => n != null && n.Length >= testValue.Length,
                "broken");

            bool? lastVal = null;

            var obs = validation
                .ValidationStatusChange
                .Subscribe(v => lastVal = v.IsValid);

            Assert.False(validation.IsValid);
            Assert.False(lastVal);
            Assert.True(lastVal.HasValue);

            model.Name = testValue + "-" + testValue;

            Assert.True(validation.IsValid);
            Assert.True(lastVal);
        }

        [Fact]
        public void PropertyContentsProvidedToMessageTest()
        {
            const string testValue = "bongo";

            var model = new TestViewModel();

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                vm => vm.Name,
                n => n != null && n.Length > testValue.Length,
                v => $"The value '{v}' is incorrect");

            model.Name = testValue;

            Assert.Equal("The value 'bongo' is incorrect", validation.Text.ToSingleLine());
        }


        /// <summary>
        /// Verify that validation message updates are correctly propagated.
        /// </summary>
        [Fact]
        public void MessageUpdatedWhenPropertyChanged()
        {
            const string testRoot = "bon";
            const string testValue = testRoot + "go";

            var model = new TestViewModel();

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                vm => vm.Name,
                n => n != null && n.Length > testValue.Length,
                v => $"The value '{v}' is incorrect");

            model.Name = testValue;

            var changes = new List<ValidationState>();

            validation.ValidationStatusChange.Subscribe(v => changes.Add(v));

            Assert.Equal("The value 'bongo' is incorrect", validation.Text.ToSingleLine());
            Assert.Single(changes);
            Assert.Equal(new ValidationState(false, "The value 'bongo' is incorrect", validation), changes[0],
                new ValidationStateComparer());

            model.Name = testRoot;

            Assert.Equal("The value 'bon' is incorrect", validation.Text.ToSingleLine());
            Assert.Equal(2, changes.Count);
            Assert.Equal(new ValidationState(false, "The value 'bon' is incorrect", validation), changes[1],
                new ValidationStateComparer());
        }

        [Fact]
        public void DualStateMessageTest()
        {
            const string testRoot = "bon";
            const string testValue = testRoot + "go";

            var model = new TestViewModel {Name = testValue};

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                vm => vm.Name,
                n => n != null && n.Length > testRoot.Length,
                (p, v) => v ? "cool" : $"The value '{p}' is incorrect");

            Assert.Equal("cool", validation.Text.ToSingleLine());

            model.Name = testRoot;

            Assert.Equal("The value 'bon' is incorrect", validation.Text.ToSingleLine());
        }


        private TestViewModel CreateDefaultValidModel()
        {
            return new TestViewModel {Name = "name"};
        }
    }

    public class TestViewModel : ReactiveObject, ISupportsValidation
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        private string _name2;

        public string Name2
        {
            get => _name2;
            set => this.RaiseAndSetIfChanged(ref _name2, value);
        }

        public List<string> GetIt { get; set; } = new List<string>();

        public string Go()
        {
            return "here";
        }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}