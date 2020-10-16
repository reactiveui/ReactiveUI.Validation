[![NuGet Stats](https://img.shields.io/nuget/v/reactiveui.validation.svg)](https://www.nuget.org/packages/reactiveui.validation) ![Build](https://github.com/reactiveui/ReactiveUI.Validation/workflows/Build/badge.svg) [![Code Coverage](https://codecov.io/gh/reactiveui/ReactiveUI.Validation/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI.Validation) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute) [![Downloads](https://img.shields.io/nuget/dt/reactiveui.validation.svg)](https://www.nuget.org/packages/reactiveui.validation) [![Slack](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)

<a href="https://github.com/reactiveui/ReactiveUI.Validation">
  <img width="140" heigth="140" src="https://github.com/reactiveui/ReactiveUI.Validation/blob/main/media/logo.png">
</a>

# ReactiveUI.Validation

Validation for ReactiveUI based solutions, functioning in a reactive way. Based on [jcmm33's Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation).

## NuGet Packages

Install the following package into you class library and platform-specific project. ReactiveUI.Validation package supports all platforms, including .NET Framework, .NET Standard, .NET Core, MonoAndroid, Tizen, UAP, Xamarin.iOS, Xamarin.Mac, Xamarin.TVOS.

| Platform          | ReactiveUI Package                  | NuGet                |
| ----------------- | ----------------------------------- | -------------------- |
| Any               | [ReactiveUI.Validation][CoreDoc]    | [![CoreBadge]][Core] |

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

## How to Use

* For ViewModels which need validation, implement `IValidatableViewModel`.
* Add validation rules to the ViewModel using the `ValidationRule` extension methods.
* Bind to the validation rules in the View via `BindValidation` or `INotifyDataErrorInfo`.

## Example

1. Decorate existing ViewModel with `IValidatableViewModel`, which has a single member, `ValidationContext`. The ValidationContext contains all of the functionality surrounding the validation of the ViewModel. Most access to the specification of validation rules is performed through extension methods on the `IValidatableViewModel` interface. Then, add validation to the ViewModel.

```csharp
public class SampleViewModel : ReactiveObject, IValidatableViewModel
{
    public SampleViewModel()
    {
        // Creates the validation for the Name property.
        this.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "You must specify a valid name");
    }

    public ValidationContext ValidationContext { get; } = new ValidationContext();

    private string _name;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

For more complex validations there is also the possibility to supply an `IObservable<bool>` indicating whether the `ValidationRule` is valid or not. Thus you can combine multiple properties or incorporate other complex logic.

```csharp
IObservable<bool> passwordsObservable =
    this.WhenAnyValue(
        x => x.Password,
        x => x.ConfirmPassword,
        (password, confirmation) => password == confirmation);

this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    "Passwords must match.");
```

Also, we support validations for arbitrary `IObservable<TState>` streams of events. The `ValidationRule` overload that works with `IObservable<TState>` gives you the ability to specify a custom validation function that depends on `TState`, and a custom error message function, responsible for formatting the `TState` object. This means your `IObservable<TState>` can even make a call to a API endpoint internally. The syntax for this looks as such:

```csharp
// IObservable<{ Password, Confirmation }>
var passwordsObservable =
    this.WhenAnyValue(
        x => x.Password,
        x => x.ConfirmPassword,
        (password, confirmation) =>
            new { Password = password, Confirmation = confirmation });

this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    state => state.Password == state.Confirmation,
    state => $"Passwords must match: {state.Password} != {state.Confirmation}");
```

Finally, you can directly supply any state that implements `IValidationState`; or you can use the `ValidationState` base class which already implements the interface.  As the resulting object is stored directly against the context without further transformation, this can be the most performant approach:
```csharp
IObservable<IValidationState> usernameNotEmpty =
    this.WhenAnyValue(x => x.UserName)
        .Select(name => string.IsNullOrEmpty(name) 
            ? new ValidationState(false, "The username must not be empty")
            : ValidationState.Valid);

this.ValidationRule(vm => vm.UserName, usernameNotEmpty);
```

2. Add validation presentation to the View.

```csharp
public class SampleView : ReactiveContentPage<SampleViewModel>
{
    public SampleView()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            this.Bind(ViewModel, vm => vm.Name, view => view.Name.Text)
                .DisposeWith(disposables);

            // Bind any validations that reference the Name property 
            // to the text of the NameError UI control.
            this.BindValidation(ViewModel, vm => vm.Name, view => view.NameError.Text)
                .DisposeWith(disposables);

            // Bind any validations attached to this particular view model
            // to the text of the FormErrors UI control.
            this.BindValidation(ViewModel, view => view.FormErrors.Text)
                .DisposeWith(disposables);
        });
    }
}
```

> **Note** `Name` is an `<Entry />`, `NameError` is a `<Label />`, and `FormErrors` is a `<Label />` as well. All these controls are from the [Xamarin.Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/) library.

## Example with Android Extensions

There are extensions methods for Xamarin.Android and its Material design control `TextInputLayout`. These extensions use internally the Error property from the `TextInputLayout` control, allowing you to implement a fully native behavior to showing validation errors. To use these extensions you must import `ReactiveUI.Validation.Platforms.Android`:

```csharp
// This using directive makes Android-specific extensions available.
using ReactiveUI.Validation.Platforms.Android;

namespace SampleApp.Activities
{
    public class SampleActivity : ReactiveAppCompatActivity<SampleViewModel>
    {
        public TextInputEditText Name { get; set; }

        public TextInputLayout NameLayout { get; set; }

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            // The WireUpControls method is a magic ReactiveUI utility method for Android, see:
            // https://www.reactiveui.net/docs/handbook/data-binding/xamarin-android/wire-up-controls
            this.WireUpControls();

            this.Bind(ViewModel, vm => vm.Name, view => view.Name.Text);

            // Bind any validations which reference the Name property 
            // to the Error property of the TextInputLayout control.
            this.BindValidation(ViewModel, vm => vm.Name, NameLayout);
        }
    }
}
```

## `INotifyDataErrorInfo` Support

For those platforms that support the `INotifyDataErrorInfo` interface, ReactiveUI.Validation provides a helper base class named `ReactiveValidationObject`. The helper class implements both the `IValidatableViewModel` interface and the `INotifyDataErrorInfo` interface. It listens to any changes in the `ValidationContext` and invokes `INotifyDataErrorInfo` events. 

```cs
public class SampleViewModel : ReactiveValidationObject
{
    public SampleViewModel()
    {
        this.ValidationRule(
            viewModel => viewModel.Name, 
            name => !string.IsNullOrWhiteSpace(name),
            "Name shouldn't be null or white space.");
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

When using the `ValidationRule` overload that uses `IObservable<bool>` for more complex scenarios please remember to supply the property which the validation rule is targeting as the first argument. Otherwise it is not possible for `INotifyDataErrorInfo` to conclude which property the error message is for.

```csharp
this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    "Passwords must match.");
```

## Capabilities

In essence, ReactiveUI.Validation is a relatively simple model of the `ValidationContext` containing a list of `IValidationComponent` instances. An `IValidationComponent` provides an observable for `ValidationState`. Whenever validation state changes (either a transition of validity) or `ValidationText` changes, then a new value is pushed out.

1. Rules can be composed of single or multiple properties along with more generic Observables.
2. Validation text can encapsulate both valid and invalid states.
3. Binding can occur to either a View or an action.
4. Validation text can reference either the ViewModel or properties which comprise the validation rule e.g. include text entered as part of validation message.
5. Validation text output can be adjusted using custom formatters, not only allowing for single & multiline output but also for platforms like Android it should be possible to achieve richer renderings i.e. Bold/italics.

```cs
// This formatter is based on the default SingleLineFormatter but uses a custom separator char.
var formatter = new SingleLineFormatter(Environment.NewLine);
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

The simplest possible `IValidationTextFormatter<TOut>` implementation may look like this one.

```cs
private class ConstFormatter : IValidationTextFormatter<string>
{
    private readonly string _text;

    public ConstFormatter(string text = "The input is invalid.") => _text = text;

    public string Format(ValidationText validationText) => _text;
}

// This formatter is based on a custsom IValidationTextFormatter implementation.
var formatter = new ConstFormatter("The input is invalid.");
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

## Feedback

Please use [GitHub issues](https://github.com/reactiveui/ReactiveUI.Validation/issues) for questions, comments, or bug reports.

## Authors

* **jcmm33** - *Initial work* - [GitHub profile](https://github.com/jcmm33)
* **Àlex Martínez Morón** - *Repository maintenance* - [GitHub profile](https://github.com/alexmartinezm)

## Contribute

ReactiveUI.Validation is developed under an OSI-approved open source license, making it freely usable and distributable, even for commercial use. Because of our Open Collective model for funding and transparency, we are able to funnel support and funds through to our contributors and community. We ❤ the people who are involved in this project, and we’d love to have you on board, especially if you are just getting started or have never contributed to open-source before.

So here's to you, lovely person who wants to join us — this is how you can support us:

* [Responding to questions on StackOverflow](https://stackoverflow.com/questions/tagged/reactiveui)
* [Passing on knowledge and teaching the next generation of developers](http://ericsink.com/entries/dont_use_rxui.html)
* [Donations](https://reactiveui.net/donate) and [Corporate Sponsorships](https://reactiveui.net/sponsorship)
* [Asking your employer to reciprocate and contribute to open-source](https://github.com/github/balanced-employee-ip-agreement)
* Submitting documentation updates where you see fit or lacking.
* Making contributions to the code base.

## Copyright and License

Code released under the [MIT license](https://opensource.org/licenses/MIT).
