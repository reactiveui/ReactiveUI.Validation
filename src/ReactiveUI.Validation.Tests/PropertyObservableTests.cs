using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Validation.Comparators;
using ReactiveUI.Validation.Components;
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
                (n) => !string.IsNullOrEmpty(n),"broken");

            Assert.True(validation.IsValid);
            Assert.True(string.IsNullOrEmpty(validation.Text.ToSingleLine()));
        }

        [Fact]
        public void StateTransitionsWhenValidityChangesTest()
        {
            var model = new TestViewModel();

            var testValue = "test";

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                                                                    vm => vm.Name,
                (n) => n != null && n.Length >= testValue.Length, "broken");

            bool? lastVal = null;

            var obs = validation.ValidationStatusChange.Subscribe(v => lastVal = v.IsValid);

            Assert.False(validation.IsValid);
            Assert.False(lastVal);
            Assert.True(lastVal.HasValue);

            model.Name = testValue+"-"+testValue;

            Assert.True(validation.IsValid);
            Assert.True(lastVal);
        }

        [Fact]
        public void PropertyContentsProvidedToMessageTest()
        {
            var model = new TestViewModel();

            var testValue = "bongo";

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                                                                    vm => vm.Name,
                (n) => n != null && n.Length > testValue.Length, (v) => $"The value '{v}' is incorrect");

            model.Name = testValue;

            var i = validation.IsValid;

            Assert.Equal("The value 'bongo' is incorrect", validation.Text.ToSingleLine());
        }



        /// <summary>
        /// Verify that validation message updates are correctly propogated.
        /// </summary>
        [Fact]
        public void MessageUpdatedWhenPropertyChanged()
        {
            var model = new TestViewModel();

            var testRoot = "bon";
            var testValue = testRoot+"go";
            

            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                                                                    vm => vm.Name,
                (n) => n != null && n.Length > testValue.Length, (v) => $"The value '{v}' is incorrect");

            model.Name = testValue;

            var i = validation.IsValid;

            List<ValidationState> changes = new List<ValidationState>();

            validation.ValidationStatusChange.Subscribe(v => changes.Add(v));

            Assert.Equal("The value 'bongo' is incorrect", validation.Text.ToSingleLine());
            Assert.Equal(1,changes.Count);
            Assert.Equal(new ValidationState(false, "The value 'bongo' is incorrect", validation), changes[0], new ValidationStateComparer());

            model.Name = testRoot;

            Assert.Equal("The value 'bon' is incorrect", validation.Text.ToSingleLine());
            Assert.Equal(2, changes.Count);
            Assert.Equal(new ValidationState(false, "The value 'bon' is incorrect",validation),changes[1],new ValidationStateComparer() );
        }

        [Fact]
        public void DualStateMessageTest()
        {
            var testRoot = "bon";
            var testValue = testRoot + "go";

            var model = new TestViewModel() {Name = testValue};


            var validation = new BasePropertyValidation<TestViewModel, string>(model,
                                                                    vm => vm.Name,
                (n) => n != null && n.Length > testRoot.Length, (p,v) => v ? "cool" : $"The value '{p}' is incorrect");

            Assert.Equal("cool",validation.Text.ToSingleLine());

            model.Name = testRoot;

            Assert.Equal("The value 'bon' is incorrect", validation.Text.ToSingleLine());

        }


        private TestViewModel CreateDefaultValidModel()
        {
            return new TestViewModel() {Name = "name"};

        }
    }

    public class TestViewModel : ReactiveObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _name2;

        public string Name2
        {
            get { return _name2; }
            set { this.RaiseAndSetIfChanged(ref _name2, value); }
        }

        public List<string> GetIt { get; set; } = new List<string>();

        public string Go()
        {
            return "here";
        }
    }
}