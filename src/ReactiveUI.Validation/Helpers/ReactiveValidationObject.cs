using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace ReactiveUI.Validation.Helpers
{
    public abstract class ReactiveValidationObject<TViewModel> : ReactiveObject, IValidatableViewModel, INotifyDataErrorInfo
    {
        private readonly ObservableAsPropertyHelper<bool> _hasErrors;
    
        protected ReactiveValidationObject(IScheduler scheduler = null)
        {
            ValidationContext = new ValidationContext(scheduler);
        
            _hasErrors = this
                .IsValid()
                .Select(valid => !valid)
                .ToProperty(this, x => x.HasErrors, scheduler: scheduler);
        
            ValidationContext
                .ValidationStatusChange
                .CombineLatest(Changed, (validation, change) => change.PropertyName)
                .Where(name => name != nameof(HasErrors))
                .Select(name => new DataErrorsChangedEventArgs(name))
                .Subscribe(args => ErrorsChanged?.Invoke(this, args)); 
        }

        public bool HasErrors => _hasErrors.Value;
    
        public ValidationContext ValidationContext { get; }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    
        public IEnumerable GetErrors(string propertyName)
        {
            var memberInfoName = GetType()
                .GetMember(propertyName)
                .FirstOrDefault()?
                .ToString();
            
            if (memberInfoName == null) 
                return Enumerable.Empty<string>();
            
            var relatedPropertyValidations = ValidationContext
                .Validations
                .OfType<BasePropertyValidation<TViewModel>>()
                .Where(validation => validation.ContainsPropertyName(memberInfoName));
                
            return relatedPropertyValidations
                .Where(validation => !validation.IsValid)
                .SelectMany(validation => validation.Text)
                .ToList();
        }
    }
}
