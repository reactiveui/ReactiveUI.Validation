# Validate a checkout form

The checkout example requires a delivery name and a positive quantity. It reads the error for each field, formats the quantity message, corrects the form, then removes the quantity rule. Both projects use the same [view model](Checkout/CheckoutForm.cs) and [checks](Checkout/Program.cs).

| Package | Run from `src` |
| --- | --- |
| ReactiveUI.Validation | `dotnet run --project examples/Checkout -f net11.0` |
| ReactiveUI.Validation.Reactive | `dotnet run --project examples/Checkout.Reactive -f net11.0` |

Use `net10.0` to run on .NET 10. The [signup samples](../samples/LoginApp) include a shared view model with an asynchronous rule and views for desktop and mobile platforms.
