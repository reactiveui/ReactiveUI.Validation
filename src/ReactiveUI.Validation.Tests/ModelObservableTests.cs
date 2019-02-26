using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Validation.Components;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    public class ModelObservableTests
    {
        [Fact]
        public void InitialValidStateIsCorrect()
        {
            var model = new TestViewModel() {Name = "name", Name2 = "name2"};

            Subject<bool> validState = new Subject<bool>();

            var v = new ModelObservableValidation<TestViewModel>(model,(m) => validState.StartWith(true),(m,s) => "broken") ;

            Assert.True(v.IsValid);
        }

        [Fact]
        public void ObservableToInvalid()
        {
            var model = new TestViewModel() { Name = "name", Name2 = "name2" };

            ReplaySubject<bool> validState = new ReplaySubject<bool>(1);

            var v = new ModelObservableValidation<TestViewModel>(model, (m) => validState, (m,s) => "broken");

            validState.OnNext(false);
            validState.OnNext(true);
            validState.OnNext(false);

            Assert.False(v.IsValid);
            Assert.Equal("broken",v.Text.ToSingleLine());
        }
    }
}