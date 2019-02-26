using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
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
            Assert.Equal(0,vc.Text.Count);               
        }

        [Fact]
        public void CanAddValidationComponentsTest()
        {
            var vc = new ValidationContext();

            var validName = "valid";
            var invalidName = String.Empty;

            var vm = new TestViewModel() {Name = "valid"};

            var v1 = new BasePropertyValidation<TestViewModel, string>(vm, v => v.Name,
                (s) => !string.IsNullOrEmpty(s), s => $"{s} isn't valid");

            vc.Add(v1);

            Assert.True(vc.IsValid);

            vm.Name = invalidName;

            Assert.False(v1.IsValid);
            Assert.False(vc.IsValid);

            Assert.Equal(1,vc.Text.Count);
        }

        [Fact]
        public void TwoValidationComponentsCorrectlyResultInContextTest()
        {
            var vc = new ValidationContext();

            var validName = "valid";
            var invalidName = String.Empty;

            var vm = new TestViewModel() { Name = validName,Name2=validName };

            var v1 = new BasePropertyValidation<TestViewModel, string>(vm, v => v.Name,
                (s) => !string.IsNullOrEmpty(s), s => $"Name {s} isn't valid");

            var v2 = new BasePropertyValidation<TestViewModel, string>(vm, v => v.Name2,
                (s) => !string.IsNullOrEmpty(s), s => $"Name 2 {s} isn't valid");

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
            Assert.Equal("Name "+invalidName+" isn't valid",vc.Text[0]);
            Assert.Equal("Name 2 " + invalidName + " isn't valid", vc.Text[1]);

            vm.Name = validName;
            vm.Name2 = validName;

            Assert.True(vc.IsValid);
            Assert.Equal(0,vc.Text.Count);
        }
    }
}