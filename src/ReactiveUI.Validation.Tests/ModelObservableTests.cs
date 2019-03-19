using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for <see cref="ModelObservableValidation{TViewModel}"/>.
    /// </summary>
    public class ModelObservableTests
    {
        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateIsCorrectTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };

            var validState = new Subject<bool>();

            var v = new ModelObservableValidation<TestViewModel>(
                model,
                _ => validState.StartWith(true),
                (_, __) => "broken");

            Assert.True(v.IsValid);
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };

            var validState = new ReplaySubject<bool>(1);

            var v = new ModelObservableValidation<TestViewModel>(
                model,
                _ => validState,
                (_, __) => "broken");

            validState.OnNext(false);
            validState.OnNext(true);
            validState.OnNext(false);

            Assert.False(v.IsValid);
            Assert.Equal("broken", v.Text.ToSingleLine());
        }
    }
}