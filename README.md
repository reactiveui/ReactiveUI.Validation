<div align="center">

<a href="https://github.com/reactiveui/ReactiveUI.Validation"><img src="https://raw.githubusercontent.com/reactiveui/ReactiveUI.Validation/main/media/logo.png" alt="ReactiveUI.Validation" width="128"></a>

# ReactiveUI.Validation

[![Build](https://img.shields.io/github/actions/workflow/status/reactiveui/ReactiveUI.Validation/ci-build.yml?branch=main&label=build&logo=githubactions&logoColor=white)](https://github.com/reactiveui/ReactiveUI.Validation/actions/workflows/ci-build.yml)
[![NuGet](https://img.shields.io/nuget/v/ReactiveUI.Validation?label=nuget&logo=nuget&logoColor=white&color=004880)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Downloads](https://img.shields.io/nuget/dt/ReactiveUI.Validation?label=downloads&logo=nuget&logoColor=white&color=004880)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Coverage](https://img.shields.io/codecov/c/github/reactiveui/ReactiveUI.Validation?label=coverage&logo=codecov&logoColor=white)](https://codecov.io/gh/reactiveui/ReactiveUI.Validation)
[![License](https://img.shields.io/github/license/reactiveui/ReactiveUI.Validation?color=blue)](https://github.com/reactiveui/ReactiveUI.Validation/blob/main/LICENSE)
[![Discussions](https://img.shields.io/badge/discussions-ask%20a%20question-8A2BE2?logo=github&logoColor=white)](https://github.com/reactiveui/ReactiveUI.Validation/discussions)

</div>

Validation for ReactiveUI based solutions, functioning in a reactive way.

*Read this in other languages: [Español](README.es.md).*

## Core Team

<table>
  <tbody>
    <tr>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/ChrisPulman.png?s=150">
        <br>
        <a href="https://github.com/ChrisPulman">Chris Pulman</a>
        <p>London, UK</p>
      </td>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/glennawatson.png?s=150">
        <br>
        <a href="https://github.com/glennawatson">Glenn Watson</a>
        <p>Melbourne, Australia</p>
      </td>
    </tr>
  </tbody>
</table>

## Alumni & History

ReactiveUI.Validation was originally developed by [@jcmm33](https://github.com/jcmm33) as [Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation), and then refactored and updated by [Àlex Martínez Morón](https://github.com/alexmartinezm) and the ReactiveUI community.

## Supported Platforms

| Platform | Targets |
|----------|---------|
| .NET | net10.0, net11.0 |
| .NET Framework | net47, net471, net472, net48, net481 |
| Windows | net10.0-windows10.0.19041.0, net11.0-windows10.0.19041.0 |
| Android (MAUI) | net10.0-android, net11.0-android |

## NuGet Packages

Install the following package into your class library and into a platform-specific project.

| Platform | Package | NuGet |
|----------|---------|-------|
| Any Platform | [ReactiveUI.Validation][CoreDoc] | [![CoreBadge]][Core] |
| AndroidX (MAUI) | [ReactiveUI.Validation.AndroidX][DroDoc] | [![DroXBadge]][DroX] |
| Any Platform, with `ReactiveUI.Reactive` | ReactiveUI.Validation.Reactive | [![ReactiveBadge]][Reactive] |
| AndroidX (MAUI), with `ReactiveUI.Reactive` | ReactiveUI.Validation.AndroidX.Reactive | [![DroXReactiveBadge]][DroXReactive] |

Pick the package that matches your ReactiveUI package:

- If your app uses `ReactiveUI`, install `ReactiveUI.Validation`.
- If your app uses `ReactiveUI.Reactive` (the System.Reactive flavour), install `ReactiveUI.Validation.Reactive`. On AndroidX, install `ReactiveUI.Validation.AndroidX.Reactive` as well.

`ReactiveUI.Validation.Reactive` has the same API. Its namespaces start with `ReactiveUI.Validation.Reactive` instead of `ReactiveUI.Validation`. Its schedulers are System.Reactive `IScheduler` instances.

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

[DroX]: https://www.nuget.org/packages/ReactiveUI.Validation.AndroidX/
[DroXBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.AndroidX.svg
[DroDoc]: https://github.com/reactiveui/reactiveui.validation#example-with-android-extensions

[Reactive]: https://www.nuget.org/packages/ReactiveUI.Validation.Reactive/
[ReactiveBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.Reactive.svg

[DroXReactive]: https://www.nuget.org/packages/ReactiveUI.Validation.AndroidX.Reactive/
[DroXReactiveBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.AndroidX.Reactive.svg

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

ReactiveUI.Validation is part of [ReactiveUI](https://github.com/reactiveui) and is released under the [MIT license](https://github.com/reactiveui/ReactiveUI.Validation/blob/main/LICENSE). We welcome contributions of every size, from bug reports and documentation to features and tests.

- **Questions:** ask in [GitHub Discussions](https://github.com/reactiveui/ReactiveUI.Validation/discussions).
- **Bugs and feature requests:** open a [GitHub issue](https://github.com/reactiveui/ReactiveUI.Validation/issues/new/choose).
- **Larger changes:** open an issue or discussion first, so we can agree on the approach before you start.
- **Contributor guide:** read [reactiveui.net/contribute](https://www.reactiveui.net/contribute/) for [building and testing](https://www.reactiveui.net/contribute/building-and-testing/) and [pull requests](https://www.reactiveui.net/contribute/pull-requests/).

### Coding standards

Every pull request must meet these standards. The build checks most of them.

- Follow the [ReactiveUI C# style guide](https://www.reactiveui.net/contribute/software-style-guide/code-style/).
- The repository's `.editorconfig` and the StyleSharp, PerformanceSharp and SecuritySharp analyzers run on every build. Fix each finding in the code. Do not suppress it or relax the `.editorconfig`.
- Add tests for every bug fix and new feature.
- Document public APIs with XML documentation comments.
- Write commit messages in the [Conventional Commits](https://www.reactiveui.net/contribute/software-style-guide/commit-message-convention/) format.
- Follow the [Code of Conduct](https://github.com/reactiveui/.github/blob/main/CODE_OF_CONDUCT.md).

### Support the project

- Sponsor the maintainers on GitHub Sponsors: [reactivemarbles](https://github.com/sponsors/reactivemarbles), [glennawatson](https://github.com/sponsors/glennawatson) and [chrispulman](https://github.com/sponsors/chrispulman).
- Ask your employer to support open source, for example with the [balanced employee IP agreement](https://github.com/github/balanced-employee-ip-agreement).

## Sponsors

[JetBrains](https://www.jetbrains.com/) gives ReactiveUI's maintainers licences for its tools through its
[open source support programme](https://www.jetbrains.com/community/opensource/).
[Anthropic](https://www.anthropic.com/) supports them with [Claude](https://claude.com/) through
[Claude for Open Source](https://claude.com/contact-sales/claude-for-oss).
[OpenAI](https://openai.com/) supports them with [Codex](https://openai.com/codex/) through
[Codex for Open Source](https://developers.openai.com/community/codex-for-oss).
[Snyk](https://snyk.io/) gives the maintainers access to security tools through its
[Secure Developer Program](https://snyk.io/open-source/).

[![JetBrains](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/jetbrains.svg)](https://www.jetbrains.com/)
[![Claude by Anthropic](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/claude.svg)](https://claude.com/)
[![OpenAI](https://raw.githubusercontent.com/reactiveui/website/main/docs/images/sponsors/openai.svg)](https://openai.com/codex/)
[![Sponsored by Snyk](https://raw.githubusercontent.com/reactiveui/website/e37a112f97ea2e9ea54b21bb7ed9e56df034d618/docs/images/sponsors/snyk.svg)](https://snyk.io/)

See [our sponsors](https://www.reactiveui.net/sponsors/) for more information.
JetBrains, Claude, Anthropic, OpenAI and Codex names and logos are trademarks of their respective owners.
Snyk and the Snyk logo are trademarks of Snyk Ltd.

## Copyright and License

Code released under the [MIT license](https://opensource.org/licenses/MIT).
