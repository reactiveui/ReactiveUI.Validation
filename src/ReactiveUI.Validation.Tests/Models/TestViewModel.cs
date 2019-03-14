using System.Collections.Generic;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Helpers;

namespace ReactiveUI.Validation.Tests.Models
{
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

        public ValidationHelper NameRule { get; set; }

        public List<string> GetIt { get; set; } = new List<string>();

        public string Go()
        {
            return "here";
        }

        public ValidationContext ValidationContext { get; } = new ValidationContext();
    }
}