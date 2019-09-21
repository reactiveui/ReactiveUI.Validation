using System.Reactive.Concurrency;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models
{
    /// <summary>
    /// Mocked ViewModel for INotifyDataErrorInfo testing.
    /// </summary>
    public class IndeiTestViewModel : ReactiveValidationObject<IndeiTestViewModel>
    {
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndeiTestViewModel"/> class.
        /// </summary>
        public IndeiTestViewModel()
            : base(ImmediateScheduler.Instance)
        {
        }

        /// <summary>
        /// Gets or sets get the Name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }
    }
}
