[![NuGet](https://img.shields.io/nuget/v/ReactiveUI.Validation.svg)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Downloads](https://img.shields.io/nuget/dt/ReactiveUI.Validation.svg)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Build](https://github.com/reactiveui/ReactiveUI.Validation/actions/workflows/ci-build.yml/badge.svg)](https://github.com/reactiveui/ReactiveUI.Validation/actions/workflows/ci-build.yml)
[![Code Coverage](https://codecov.io/gh/reactiveui/ReactiveUI.Validation/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI.Validation)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Slack](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)
[![Good First Issues](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/reactiveui/ReactiveUI.Validation/labels/good%20first%20issue)
[![Stars](https://img.shields.io/github/stars/reactiveui/ReactiveUI.Validation.svg?style=social)](https://github.com/reactiveui/ReactiveUI.Validation/stargazers)

<a href="https://github.com/reactiveui/ReactiveUI.Validation">
  <img width="140" heigth="140" src="https://github.com/reactiveui/ReactiveUI.Validation/blob/main/media/logo.png">
</a>

# ReactiveUI.Validation

Validation for ReactiveUI based solutions, functioning in a reactive way. `ReactiveUI.Validation` was originally developed by [@jcmm33](https://github.com/jcmm33) as [Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation), and then refactored and updated by [Àlex Martínez Morón](https://github.com/alexmartinezm) and the [ReactiveUI Core Team](https://github.com/reactiveui/ReactiveUI#core-team).

## Supported Platforms

| Platform | Targets |
|----------|---------|
| .NET | net8.0, net9.0, net10.0 |
| .NET Framework | net462, net472, net481 |
| Windows | net8.0-windows10.0.19041.0, net9.0-windows10.0.19041.0, net10.0-windows10.0.19041.0 |
| Android (MAUI) | net9.0-android, net10.0-android |

## NuGet Packages

Install the following package into your class library and into a platform-specific project.

| Platform | Package | NuGet |
|----------|---------|-------|
| Any Platform | [ReactiveUI.Validation][CoreDoc] | [![CoreBadge]][Core] |
| AndroidX (MAUI) | [ReactiveUI.Validation.AndroidX][DroDoc] | [![DroXBadge]][DroX] |

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

[DroX]: https://www.nuget.org/packages/ReactiveUI.Validation.AndroidX/
[DroXBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.AndroidX.svg
[DroDoc]: https://github.com/reactiveui/reactiveui.validation#example-with-android-extensions

## How to Use

* For ViewModels which need validation, implement `IValidatableViewModel`.
* Add validation rules to the ViewModel using the `ValidationRule` extension methods.
* Bind to the validation rules in the View via `BindValidation` or `INotifyDataErrorInfo`.

## Example

1. Decorate existing ViewModel with `IValidatableViewModel`, which has a single member, `ValidationContext`. The ValidationContext contains all of the functionality surrounding the validation of the ViewModel. Most access to the specification of validation rules is performed through extension methods on the `IValidatableViewModel` interface. Then, add validation to the ViewModel.

```csharp
using ReactiveUI.Validation.Extensions;

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

For more complex validation scenarios there are several more overloads of the `ValidationRule` extension method that accept observables.  These allow validation to occur asynchronously, and allows complex chains of observables to be combined to produce validation results.

The simplest accepts an `IObservable<bool>` where the observed boolean indicates whether the `ValidationRule` is valid or not.  The overload accepts a message which is used when the observable produces a `false` (_invalid_) result.

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

Any existing observables can be used to drive a `ValidationRule` using the extension method overload that accepts an arbitrary `IObservable<TState>` streams of events. The overload accepts a custom validation function that is supplied with the latest `TState`, and a custom error message function, responsible for formatting the latest `TState` object. The syntax for this looks as follows:

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
> **Note** The function to extract a message (`messageFunc`) is only invoked if the function to establish validity (`isValidFunc`) returns `false`, otherwise the message is set to `string.Empty`.

Finally, you can directly supply an observable that streams any object (or struct) that implements `IValidationState`; or you can use the `ValidationState` base class which already implements the interface.  As the resulting object is stored directly against the context without further transformation, this can be the most performant approach:
```csharp
IObservable<IValidationState> usernameNotEmpty =
    this.WhenAnyValue(x => x.UserName)
        .Select(name => string.IsNullOrEmpty(name)
            ? new ValidationState(false, "The username must not be empty")
            : ValidationState.Valid);

this.ValidationRule(vm => vm.UserName, usernameNotEmpty);
```

> **Note** As a valid `ValidationState` does not really require a message, there is a singleton `ValidationState.Valid` property that you are encouraged to use to indicate a valid state whenever possible, to reduce memory allocations.

2. Add validation presentation to the View.

```csharp
using ReactiveUI.Validation.Extensions;

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

## Example with Android Extensions

There are extensions methods for Android and its Material design control `TextInputLayout`. These extensions use internally the `Error` property from the `TextInputLayout` control, allowing you to implement a fully native behavior to showing validation errors. To use these extensions you must import `ReactiveUI.Validation.Extensions` and install `ReactiveUI.Validation.AndroidX`:

```
dotnet add package ReactiveUI.Validation.AndroidX
```

<img src="https://user-images.githubusercontent.com/6759207/96716730-15729480-13ae-11eb-928e-7e408b7ffac4.png" width="400" />

```csharp
// This using directive makes Android-specific extensions available.
using ReactiveUI.Validation.Extensions;

public class SignUpActivity : ReactiveAppCompatActivity<SignUpViewModel>
{
    // The Android native text boxes declared in an .axml file.
    public TextInputEditText Password { get; set; }
    public TextInputEditText ConfirmPassword { get; set; }

    // The layouts wrapping the text boxes declared in an .axml file.
    public TextInputLayout PasswordField { get; set; }
    public TextInputLayout ConfirmPasswordField { get; set; }

    protected override void OnCreate (Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.Main);

        // The WireUpControls method is a magic ReactiveUI utility method for Android, see:
        // https://www.reactiveui.net/docs/handbook/data-binding/xamarin-android/wire-up-controls
        this.WireUpControls();
        this.Bind(ViewModel, x => x.Password, x => x.Password.Text);
        this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPassword.Text);

        // Bind any validations which reference the Password property
        // to the Error property of the TextInputLayout control.
        this.BindValidation(ViewModel, x => x.Password, PasswordField);
        this.BindValidation(ViewModel, x => x.ConfirmPassword, ConfirmPasswordField);
    }
}
```

## `INotifyDataErrorInfo` Support

For those platforms that support the `INotifyDataErrorInfo` interface, ReactiveUI.Validation provides a helper base class named `ReactiveValidationObject`. The helper class implements both the `IValidatableViewModel` interface and the `INotifyDataErrorInfo` interface. It listens to any changes in the `ValidationContext` and invokes `INotifyDataErrorInfo` events.

<img width="400" src="https://user-images.githubusercontent.com/6759207/96717163-bbbe9a00-13ae-11eb-8c54-89cd339cbd5c.png">

```cs
using ReactiveUI.Validation.Extensions;

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

> **Note** Keep in mind that `INotifyDataErrorInfo` is only supported via XAML binding. ReactiveUI binding doesn't use the inbuilt classes of WPF.

When using a `ValidationRule` overload that accepts an observable, please remember to supply the property which the validation rule is targeting as the first argument. Otherwise it is not possible for `INotifyDataErrorInfo` to conclude which property the error message is for.

```csharp
this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    "Passwords must match.");
```

## Custom Formatters

You can pass an instance of `IValidationTextFormatter<T>` to a call to `BindValidation` if you'd like to override the default `SingleLineFormatter` used in the validation library. The `SingleLineFormatter` accepts a separator char and uses whitespace by default, so the code snippet below shows how to use a non-default separator char:

```cs
// This formatter is based on the default SingleLineFormatter but uses a custom separator char.
var formatter = new SingleLineFormatter(Environment.NewLine);
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

The simplest possible custom `IValidationTextFormatter<TOut>` implementation may look like this one.

```cs
private class ConstFormatter : IValidationTextFormatter<string>
{
    private readonly string _text;

    public ConstFormatter(string text = "The input is invalid.") => _text = text;

    public string Format(ValidationText validationText) => _text;
}

// This formatter is based on a custom IValidationTextFormatter implementation.
var formatter = new ConstFormatter("The input is invalid.");
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

If you'd like to override the `IValidationTextFormatter<string>` used in ReactiveUI.Validation by default, register an instance of `IValidationTextFormatter<string>` into `Locator.CurrentMutable` before your app starts. This could be useful in cases when your app needs localization and you wish to pass message keys instead of messages to `ValidationRule` calls.

```cs
// Register a singleton instance of IValidationTextFormatter<string> into Splat.Locator.
Locator.CurrentMutable.RegisterConstant(new CustomFormatter(), typeof(IValidationTextFormatter<string>));
```

## Capabilities

In essence, ReactiveUI.Validation is a relatively simple model of the `ValidationContext` containing a list of `IValidationComponent` instances. An `IValidationComponent` provides an observable of `IValidationState`. Whenever validation state changes (either a transition of validity) or `ValidationText` changes, then a new value is pushed out.

1. Rules can be composed of single or multiple properties along with more generic Observables.
2. Validation text can encapsulate both valid and invalid states.
3. Binding can occur to either a View or an action.
4. Validation text can reference either the ViewModel or properties which comprise the validation rule e.g. include text entered as part of validation message.
5. Validation text output can be adjusted using custom formatters, not only allowing for single & multiline output but also for platforms like Android it should be possible to achieve richer renderings i.e. Bold/italics.

## Contribute

ReactiveUI.Validation is developed under an OSI-approved open source license, making it freely usable and distributable, even for commercial use. We love the people who are involved in this project, and we'd love to have you on board, especially if you are just getting started or have never contributed to open-source before.

So here's to you, lovely person who wants to join us — this is how you can support us:

* [Responding to questions on GitHub Discussions](https://github.com/reactiveui/ReactiveUI.Validation/discussions)
* [Passing on knowledge and teaching the next generation of developers](http://ericsink.com/entries/dont_use_rxui.html)
* Submitting documentation updates where you see fit or lacking.
* Making contributions to the code base.

## Copyright and License

Code released under the [MIT license](https://opensource.org/licenses/MIT).
