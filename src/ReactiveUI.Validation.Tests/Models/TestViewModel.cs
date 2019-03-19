using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Mocked ViewModel.
    /// </summary>
    public class TestViewModel : ReactiveObject, ISupportsValidation
    {
        private string _name;
        private string _name2;

        /// <summary>
        /// Gets or sets get the Name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        /// <summary>
        /// Gets or sets get the Name2.
        /// </summary>
        public string Name2
        {
            get => _name2;
            set => this.RaiseAndSetIfChanged(ref _name2, value);
        }

        /// <summary>
        /// Gets or sets the rule of Name property.
        /// </summary>
        public ValidationHelper NameRule { get; set; }

        /// <inheritdoc/>
        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}