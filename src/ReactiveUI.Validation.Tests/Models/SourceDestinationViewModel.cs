using System.Reactive.Concurrency;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Mocked SourceDestinationViewModel.
    /// </summary>
    public class SourceDestinationViewModel : ReactiveObject, IValidatableViewModel
    {
        private TestViewModel _source = new TestViewModel();
        private TestViewModel _destination = new TestViewModel();

        /// <summary>
        /// Gets or sets get the Name.
        /// </summary>
        public TestViewModel Source
        {
            get => _source;
            set => this.RaiseAndSetIfChanged(ref _source, value);
        }

        /// <summary>
        /// Gets or sets get the Name2.
        /// </summary>
        public TestViewModel Destination
        {
            get => _destination;
            set => this.RaiseAndSetIfChanged(ref _destination, value);
        }

        /// <inheritdoc/>
        public ValidationContext ValidationContext { get; } = new ValidationContext(Scheduler.Immediate);
    }
}
