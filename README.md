# ReactiveUI.Validation

Validation for ReactiveUI based solutions, functioning in a reactive way. This repository is based on [jcmm33's Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation).

## NuGet Packages

Install the following package into you class library and platform-specific project. ReactiveUI.Validation package supports all platforms, including .NET Framework, .NET Standard, MonoAndroid, Tizen, UAP, Xamarin.iOS, Xamarin.Mac, Xamarin.TVOS.

| Platform          | ReactiveUI Package                  | NuGet                |
| ----------------- | ----------------------------------- | -------------------- |
| Any               | [ReactiveUI.Validation][CoreDoc]    | [![CoreBadge]][Core] |

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

## How to use

* For those ViewModels which need validation, implement `ISupportsValidation`
* Add validation rules to the ViewModel
* Bind to the validation rules in the View

## Example

1. Decorate existing ViewModel with `ISupportsValidation`, which has a single member, `ValidationContext`. The ValidationContext contains all of the functionality surrounding the validation of the ViewModel. Most access to the specification of validation rules is performed through extension methods on the ISupportsValidation interface. Then, add validation to the view model.

```cs
public class SampleViewModel : ReactiveObject, ISupportsValidation
{
    public SampleViewModel()
    {
        // Name must be at least 3 chars. The selector is the property 
        // name and the line below is a single property validator.
        this.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "You must specify a valid name");

        // Age must be between 13 and 100, message includes the silly 
        // length being passed in, stored in a property of the ViewModel.
        AgeRule = this.ValidationRule(
            viewModel => viewModel.Age,
            age => age >= 13 && age <= 100,
            age => $"{age} is a silly age");

        var nameAndAgeValid = this
            .WhenAnyValue(x => x.Age, x => x.Name, (age, name) => new { Age = age, Name = name })
            .Select(x => x.Age > 10 && !string.IsNullOrEmpty(x.Name));

        // Create a rule using an IObservable.
        // Store in a property of the ViewModel.
        ComplexRule = this.ValidationRule(
            _ => nameAndAgeValid,
            (vm, state) => !state ? "That's a ridiculous name / age combination" : string.Empty);
            
        // Save command is only active when all validators are valid.
        Save = ReactiveCommand.CreateFromTask(async unit => { }, this.IsValid());
    }

    public ValidationContext ValidationContext => new ValidationContext();

    public ValidationHelper ComplexRule { get; set; }

    public ValidationHelper AgeRule { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }

    private int _age;
    public int Age
    {
        get => _age;
        set => this.RaiseAndSetIfChanged(ref _age, value);
    }

    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

2. Add validation presentation to the View.

```cs
public class MainActivity : ReactiveAppCompatActivity<SampleViewModel>
{
    public EditText nameEdit { get; set; }

    public EditText ageEdit { get; set; }

    public TextView nameValidation { get; set; }

    public TextView ageValidation { get; set; }

    public TextView validationSummary { get; set; }

    public Button myButton { get; set; }

    public TextInputLayout til { get; set; }

    public TextInputEditText tiet { get; set; }

    protected override void OnCreate(Bundle bundle)
    {
        base.OnCreate(bundle);

        // Set our View from the "main" layout resource.
        SetContentView(Resource.Layout.Main);
        WireUpControls();

        this.BindCommand(ViewModel, vm => vm.Save, v => v.myButton);
        this.Bind(ViewModel, vm => vm.Name, v => v.nameEdit.Text);
        this.Bind(ViewModel, vm => vm.Age, v => v.ageEdit.Text);

        // Bind any validations which reference the name property to the text of the nameValidation control
        this.BindValidation(ViewModel, vm => vm.Name, v => v.nameValidation.Text);

        // Bind the validation specified by the AgeRule to the text of the ageValidation control
        this.BindValidation(ViewModel, vm => vm.AgeRule, v => v.ageValidation.Text);

        // bind the summary validation text to the validationSummary control
        this.BindValidation(ViewModel, v => v.validationSummary.Text);

        // bind to an Android TextInputLayout control, utilising the Error property
        this.BindValidation(ViewModel, vm => vm.ComplexRule, til);
    }
}
```

## Capabilities

1. Rules can be composed of single or multiple properties along along with more generic Observables.
2. Validation text can encapsulate both valid and invalid states.
3. Binding can occur to either a View or an action.
3. Validation text can reference either the ViewModel or properties which comprise the validation rule e.g. include text entered as part of validation message.
4. Validation text output can be adjusted using custom formatters, not only allowing for single & multiline output but also for platforms like Android it should be possible to achieve richer renderings i.e. Bold/italics.
5. Validation rules in the ViewModel "should" allow for binding of validation results for those environments that support binding (XAML). It should be noted that this however has not been tested.

## Outstanding

1. Binding hasn't been tried at all with Xamarin Forms.
2. An Android specific binding for TextInputLayout has been created, but no platform specific bindings for iOS have been created.
3. Significant usage!
4. Possible additional bit of work related to validationContext & binding.
5. Possible tidyup regarding setup of bindable ValidationHelper in ViewModel. 

## How it Works

In essence, its a relatively simple model of the ValidationContext containing a list of IValidationComponent instances. An IValidationComponent provides an observable for ValidationState. Whenever validation state changes (either a transition of validity) or ValidationText changes, then a new value is pushed out.

## Feedback

Please use [GitHub issues](https://github.com/reactiveui/ReactiveUI.Validation/issues) for questions or comments.

## Authors

* **jcmm33** - *Initial work* - [GitHub profile](https://github.com/jcmm33)
* **Àlex Martínez** - *Repository maintenance* - [GitHub profile](https://github.com/alexmartinezm)

## Copyright and license

Code released under the [MIT license](https://opensource.org/licenses/MIT).
