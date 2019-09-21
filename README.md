[![NuGet Stats](https://img.shields.io/nuget/v/reactiveui.validation.svg)](https://www.nuget.org/packages/reactiveui.validation) [![Build Status](https://dev.azure.com/dotnet/ReactiveUI/_apis/build/status/ReactiveUI.Validation-CI)](https://dev.azure.com/dotnet/ReactiveUI/_build/latest?definitionId=11)  [![Code Coverage](https://codecov.io/gh/reactiveui/ReactiveUI.Validation/branch/master/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI.Validation) [![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute) [![Downloads](https://img.shields.io/nuget/dt/reactiveui.validation.svg)](https://www.nuget.org/packages/reactiveui.validation) [![Slack](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)

<a href="https://github.com/reactiveui/ReactiveUI.Validation">
  <img width="140" heigth="140" src="https://github.com/reactiveui/ReactiveUI.Validation/blob/master/media/logo.png">
</a>

# ReactiveUI.Validation

Validation for ReactiveUI based solutions, functioning in a reactive way.

This repository is based on [jcmm33's Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation).

## NuGet Packages

Install the following package into you class library and platform-specific project. ReactiveUI.Validation package supports all platforms, including .NET Framework, .NET Standard, MonoAndroid, Tizen, UAP, Xamarin.iOS, Xamarin.Mac, Xamarin.TVOS.

| Platform          | ReactiveUI Package                  | NuGet                |
| ----------------- | ----------------------------------- | -------------------- |
| Any               | [ReactiveUI.Validation][CoreDoc]    | [![CoreBadge]][Core] |

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

## How to use

* For ViewModels which need validation, implement `IValidatableViewModel`.
* Add validation rules to the ViewModel.
* Bind to the validation rules in the View.

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

            // Bind any validations which reference the Name property 
            // to the text of the NameError UI control.
            this.BindValidation(ViewModel, vm => vm.Name, view => view.NameError.Text)
                .DisposeWith(disposables);
        });
    }
}
```

> **Note** `Name` is an Entry and `NameError` is a Label (both are controls from the Xamarin.Forms library).

## Example with Android extensions

There are extensions methods for Android specific and its Material design control `TextInputLayout`. These extensions use internally the Error property from this control, allowing you a Material Design and fully native behavior to showing errors.

To use these extensions you must import `ReactiveUI.Validation.Platforms.Android`.

```csharp
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

            // Sets our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            WireUpControls();

            this.Bind(ViewModel, vm => vm.Name, view => view.Name.Text);

            // Bind any validations which reference the Name property 
            // to the Error property of the TextInputLayout control
            this.BindValidation(ViewModel, vm => vm.Name, NameLayout);
        }
    }
}
```

## INotifyDataErrorInfo Support

For those platforms which support the `INotifyDataErrorInfo` interface, ReactiveUI.Validation provides a helper base class named `ReactiveValidationObject<TViewModel>`. The helper class implements both the `IValidatableViewModel` interface and the `INotifyDataErrorInfo` interface. It listens to any changes in the `ValidationContext` and invokes `INotifyDataErrorInfo` events. 

```cs
public class SampleViewModel : ReactiveValidationObject<SampleViewModel>
{
    [Reactive]
    public string Name { get; set; } = string.Empty;

    public SampleViewModel()
    {
        this.ValidationRule(
            x => x.Name, 
            name => !string.IsNullOrWhiteSpace(name),
            "Name shouldn't be empty.");
    }
}
```

> **Note** The `Reactive` attribute is from the [ReactiveUI.Fody](https://reactiveui.net/docs/handbook/view-models/boilerplate-code) NuGet package.

## Capabilities

1. Rules can be composed of single or multiple properties along with more generic Observables.
2. Validation text can encapsulate both valid and invalid states.
3. Binding can occur to either a View or an action.
4. Validation text can reference either the ViewModel or properties which comprise the validation rule e.g. include text entered as part of validation message.
5. Validation text output can be adjusted using custom formatters, not only allowing for single & multiline output but also for platforms like Android it should be possible to achieve richer renderings i.e. Bold/italics.

## How it Works

In essence, it's a relatively simple model of the `ValidationContext` containing a list of `IValidationComponent` instances. An `IValidationComponent` provides an observable for `ValidationState`. Whenever validation state changes (either a transition of validity) or `ValidationText` changes, then a new value is pushed out.

## Feedback

Please use [GitHub issues](https://github.com/reactiveui/ReactiveUI.Validation/issues) for questions or comments.

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
