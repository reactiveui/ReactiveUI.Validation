[![NuGet](https://img.shields.io/nuget/v/ReactiveUI.Validation.svg)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Descargas](https://img.shields.io/nuget/dt/ReactiveUI.Validation.svg)](https://www.nuget.org/packages/ReactiveUI.Validation)
[![Build](https://github.com/reactiveui/ReactiveUI.Validation/actions/workflows/ci-build.yml/badge.svg)](https://github.com/reactiveui/ReactiveUI.Validation/actions/workflows/ci-build.yml)
[![Cobertura](https://codecov.io/gh/reactiveui/ReactiveUI.Validation/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/ReactiveUI.Validation)
[![Licencia](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![Slack](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)
[![Good First Issues](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://github.com/reactiveui/ReactiveUI.Validation/labels/good%20first%20issue)
[![Stars](https://img.shields.io/github/stars/reactiveui/ReactiveUI.Validation.svg?style=social)](https://github.com/reactiveui/ReactiveUI.Validation/stargazers)

<a href="https://github.com/reactiveui/ReactiveUI.Validation">
  <img width="140" heigth="140" src="https://github.com/reactiveui/ReactiveUI.Validation/blob/main/media/logo.png">
</a>

# ReactiveUI.Validation

Validación para soluciones basadas en ReactiveUI, funcionando de manera reactiva.

> Este documento es una traducción al español de [README.md](README.md). Si encuentras alguna discrepancia, el README en inglés es la referencia autoritativa.

## Equipo principal

<table>
  <tbody>
    <tr>
      <td align="center" valign="top">
        <img width="100" height="100" src="https://github.com/ChrisPulman.png?s=150">
        <br>
        <a href="https://github.com/ChrisPulman">Chris Pulman</a>
        <p>Londres, Reino Unido</p>
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

## Historia y contribuidores anteriores

ReactiveUI.Validation fue desarrollado originalmente por [@jcmm33](https://github.com/jcmm33) como [Vistian.Reactive.Validation](https://github.com/jcmm33/ReactiveUI.Validation), y posteriormente refactorizado y actualizado por [Àlex Martínez Morón](https://github.com/alexmartinezm) y la comunidad de ReactiveUI.

## Plataformas compatibles

| Plataforma | Targets |
|------------|---------|
| .NET | net8.0, net9.0, net10.0 |
| .NET Framework | net462, net472, net481 |
| Windows | net8.0-windows10.0.19041.0, net9.0-windows10.0.19041.0, net10.0-windows10.0.19041.0 |
| Android (MAUI) | net9.0-android, net10.0-android |

## Paquetes NuGet

Instala los siguientes paquetes en tu biblioteca de clases y en el proyecto específico de la plataforma.

| Plataforma | Paquete | NuGet |
|------------|---------|-------|
| Cualquier plataforma | [ReactiveUI.Validation][CoreDoc] | [![CoreBadge]][Core] |
| AndroidX (MAUI) | [ReactiveUI.Validation.AndroidX][DroDoc] | [![DroXBadge]][DroX] |

[Core]: https://www.nuget.org/packages/ReactiveUI.Validation/
[CoreBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.svg
[CoreDoc]: https://reactiveui.net/docs/handbook/user-input-validation/

[DroX]: https://www.nuget.org/packages/ReactiveUI.Validation.AndroidX/
[DroXBadge]: https://img.shields.io/nuget/v/ReactiveUI.Validation.AndroidX.svg
[DroDoc]: https://github.com/reactiveui/reactiveui.validation#example-with-android-extensions

## Cómo usarlo

* Para los ViewModels que necesiten validación, implementa `IValidatableViewModel`.
* Añade reglas de validación al ViewModel usando los métodos de extensión `ValidationRule`.
* Vincula las reglas de validación en la vista mediante `BindValidation` o `INotifyDataErrorInfo`.

## Ejemplo

1. Decora un ViewModel existente con `IValidatableViewModel`, que tiene un único miembro: `ValidationContext`. El `ValidationContext` contiene toda la funcionalidad relacionada con la validación del ViewModel. La mayor parte del acceso a la especificación de reglas de validación se realiza a través de métodos de extensión sobre la interfaz `IValidatableViewModel`. Luego, agrega la validación al ViewModel.

```csharp
using ReactiveUI.Validation.Extensions;

public class SampleViewModel : ReactiveObject, IValidatableViewModel
{
    public SampleViewModel()
    {
        // Crea la validación para la propiedad Name.
        this.ValidationRule(
            viewModel => viewModel.Name,
            name => !string.IsNullOrWhiteSpace(name),
            "Debes especificar un nombre válido");
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

Para escenarios de validación más complejos existen varias sobrecargas adicionales del método de extensión `ValidationRule` que aceptan observables. Estas permiten que la validación ocurra de forma asíncrona, y permiten combinar cadenas complejas de observables para producir resultados de validación.

La sobrecarga más simple acepta un `IObservable<bool>` donde el booleano observado indica si la `ValidationRule` es válida o no. Esta sobrecarga acepta un mensaje que se utiliza cuando el observable produce un resultado `false` (_inválido_).

```csharp
IObservable<bool> passwordsObservable =
    this.WhenAnyValue(
        x => x.Password,
        x => x.ConfirmPassword,
        (password, confirmation) => password == confirmation);

this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    "Las contraseñas deben coincidir.");
```

Cualquier observable existente puede usarse para alimentar una `ValidationRule` mediante la sobrecarga del método de extensión que acepta un flujo arbitrario `IObservable<TState>`. La sobrecarga acepta una función de validación personalizada que recibe el último `TState`, y una función de mensaje de error personalizada, responsable de formatear el último objeto `TState`. La sintaxis se ve así:

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
    state => $"Las contraseñas deben coincidir: {state.Password} != {state.Confirmation}");
```
> **Nota:** La función para extraer un mensaje (`messageFunc`) solo se invoca si la función que establece la validez (`isValidFunc`) devuelve `false`; en caso contrario, el mensaje se establece como `string.Empty`.

Finalmente, puedes proporcionar directamente un observable que emita cualquier objeto (o struct) que implemente `IValidationState`; o puedes usar la clase base `ValidationState`, que ya implementa la interfaz. Como el objeto resultante se almacena directamente en el contexto sin transformación adicional, este puede ser el enfoque más eficiente:
```csharp
IObservable<IValidationState> usernameNotEmpty =
    this.WhenAnyValue(x => x.UserName)
        .Select(name => string.IsNullOrEmpty(name)
            ? new ValidationState(false, "El nombre de usuario no debe estar vacío")
            : ValidationState.Valid);

this.ValidationRule(vm => vm.UserName, usernameNotEmpty);
```

> **Nota:** Dado que un `ValidationState` válido no requiere realmente un mensaje, existe una propiedad singleton `ValidationState.Valid` que se recomienda utilizar siempre que sea posible para indicar un estado válido, reduciendo así las asignaciones de memoria.

2. Añade la presentación de la validación a la vista.

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

            // Vincula cualquier validación que referencie la propiedad Name
            // al texto del control de UI NameError.
            this.BindValidation(ViewModel, vm => vm.Name, view => view.NameError.Text)
                .DisposeWith(disposables);

            // Vincula cualquier validación asociada a este ViewModel concreto
            // al texto del control de UI FormErrors.
            this.BindValidation(ViewModel, view => view.FormErrors.Text)
                .DisposeWith(disposables);
        });
    }
}
```

## Ejemplo con extensiones de Android

Existen métodos de extensión para Android y su control de Material Design `TextInputLayout`. Estas extensiones utilizan internamente la propiedad `Error` del control `TextInputLayout`, permitiendo implementar un comportamiento totalmente nativo para mostrar errores de validación. Para usar estas extensiones debes importar `ReactiveUI.Validation.Extensions` e instalar `ReactiveUI.Validation.AndroidX`:

```
dotnet add package ReactiveUI.Validation.AndroidX
```

<img src="https://user-images.githubusercontent.com/6759207/96716730-15729480-13ae-11eb-928e-7e408b7ffac4.png" width="400" />

```csharp
// Esta directiva using hace disponibles las extensiones específicas de Android.
using ReactiveUI.Validation.Extensions;

public class SignUpActivity : ReactiveAppCompatActivity<SignUpViewModel>
{
    // Los cuadros de texto nativos de Android declarados en un archivo .axml.
    public TextInputEditText Password { get; set; }
    public TextInputEditText ConfirmPassword { get; set; }

    // Los layouts que envuelven los cuadros de texto declarados en un archivo .axml.
    public TextInputLayout PasswordField { get; set; }
    public TextInputLayout ConfirmPasswordField { get; set; }

    protected override void OnCreate (Bundle bundle)
    {
        base.OnCreate(bundle);
        SetContentView(Resource.Layout.Main);

        // El método WireUpControls es un método utilitario de ReactiveUI para Android, ver:
        // https://www.reactiveui.net/docs/handbook/data-binding/xamarin-android/wire-up-controls
        this.WireUpControls();
        this.Bind(ViewModel, x => x.Password, x => x.Password.Text);
        this.Bind(ViewModel, x => x.ConfirmPassword, x => x.ConfirmPassword.Text);

        // Vincula cualquier validación que referencie la propiedad Password
        // a la propiedad Error del control TextInputLayout.
        this.BindValidation(ViewModel, x => x.Password, PasswordField);
        this.BindValidation(ViewModel, x => x.ConfirmPassword, ConfirmPasswordField);
    }
}
```

## Soporte para `INotifyDataErrorInfo`

Para aquellas plataformas que admiten la interfaz `INotifyDataErrorInfo`, ReactiveUI.Validation proporciona una clase base auxiliar llamada `ReactiveValidationObject`. Esta clase auxiliar implementa tanto la interfaz `IValidatableViewModel` como la interfaz `INotifyDataErrorInfo`. Escucha cualquier cambio en el `ValidationContext` e invoca los eventos de `INotifyDataErrorInfo`.

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
            "El nombre no debe ser nulo ni estar en blanco.");
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
}
```

> **Nota:** Ten en cuenta que `INotifyDataErrorInfo` solo se admite mediante el binding de XAML. El binding de ReactiveUI no utiliza las clases integradas de WPF.

Cuando uses una sobrecarga de `ValidationRule` que acepte un observable, recuerda proporcionar como primer argumento la propiedad a la que se dirige la regla de validación. De lo contrario, `INotifyDataErrorInfo` no puede determinar a qué propiedad pertenece el mensaje de error.

```csharp
this.ValidationRule(
    vm => vm.ConfirmPassword,
    passwordsObservable,
    "Las contraseñas deben coincidir.");
```

## Formateadores personalizados

Puedes pasar una instancia de `IValidationTextFormatter<T>` a una llamada de `BindValidation` si deseas reemplazar el `SingleLineFormatter` predeterminado utilizado en la biblioteca de validación. `SingleLineFormatter` acepta un carácter separador y usa un espacio en blanco por defecto, así que el siguiente fragmento muestra cómo usar un separador no predeterminado:

```cs
// Este formateador se basa en el SingleLineFormatter predeterminado pero usa un separador personalizado.
var formatter = new SingleLineFormatter(Environment.NewLine);
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

La implementación personalizada más simple posible de `IValidationTextFormatter<TOut>` podría verse así:

```cs
private class ConstFormatter : IValidationTextFormatter<string>
{
    private readonly string _text;

    public ConstFormatter(string text = "La entrada es inválida.") => _text = text;

    public string Format(ValidationText validationText) => _text;
}

// Este formateador se basa en una implementación personalizada de IValidationTextFormatter.
var formatter = new ConstFormatter("La entrada es inválida.");
this.BindValidation(ViewModel, x => x.ErrorLabel.Text, formatter)
    .DisposeWith(disposables);
```

Si deseas reemplazar el `IValidationTextFormatter<string>` utilizado por defecto en ReactiveUI.Validation, registra una instancia de `IValidationTextFormatter<string>` en `Locator.CurrentMutable` antes de que tu aplicación inicie. Esto puede ser útil cuando tu aplicación necesita localización y deseas pasar claves de mensajes en lugar de mensajes a las llamadas de `ValidationRule`.

```cs
// Registra una instancia singleton de IValidationTextFormatter<string> en Splat.Locator.
Locator.CurrentMutable.RegisterConstant(new CustomFormatter(), typeof(IValidationTextFormatter<string>));
```

## Capacidades

En esencia, ReactiveUI.Validation es un modelo relativamente simple del `ValidationContext` que contiene una lista de instancias `IValidationComponent`. Un `IValidationComponent` provee un observable de `IValidationState`. Cada vez que el estado de validación cambia (ya sea una transición de validez) o cambia el `ValidationText`, se emite un nuevo valor.

1. Las reglas pueden componerse de una o varias propiedades junto con observables más genéricos.
2. El texto de validación puede encapsular tanto estados válidos como inválidos.
3. La vinculación puede realizarse a una vista o a una acción.
4. El texto de validación puede referenciar al ViewModel o a las propiedades que componen la regla de validación, por ejemplo incluir texto introducido como parte del mensaje de validación.
5. La salida del texto de validación puede ajustarse usando formateadores personalizados, no solo permitiendo salida en una o varias líneas, sino también en plataformas como Android donde es posible lograr renderizados más ricos, p. ej. negrita/cursiva.

## Contribuir

ReactiveUI.Validation se desarrolla bajo una licencia de código abierto aprobada por la OSI, lo que la hace de uso y distribución libre, incluso para uso comercial. Valoramos a las personas involucradas en este proyecto, y nos encantaría tenerte a bordo, especialmente si recién estás comenzando o nunca antes has contribuido a un proyecto de código abierto.

Así que, para ti, persona estupenda que quiere unirse a nosotros, así es como puedes apoyarnos:

* [Respondiendo preguntas en GitHub Discussions](https://github.com/reactiveui/ReactiveUI.Validation/discussions)
* [Transmitiendo conocimiento y formando a la próxima generación de desarrolladores](http://ericsink.com/entries/dont_use_rxui.html)
* Enviando actualizaciones de documentación donde lo consideres apropiado o necesario.
* Realizando contribuciones al código base.

## Derechos de autor y licencia

Código publicado bajo la [licencia MIT](https://opensource.org/licenses/MIT).
