// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Checkout;

/// <summary>Validates a checkout form as the customer edits it.</summary>
internal static class Program
{
    /// <summary>Checks field errors, validation text, a corrected form and removal of a rule.</summary>
    /// <exception cref="InvalidOperationException">The observed errors or form validity differ from the rules.</exception>
    internal static void Main()
    {
        const string MissingName = "Enter a delivery name.";
        _ = RxAppBuilder.CreateReactiveUIBuilder().WithCoreServices().BuildApp();
        using var form = new CheckoutForm();
        using var name = form.ValidationRule(x => x.Name, static value => !string.IsNullOrWhiteSpace(value), MissingName);
        using var quantity = form.ValidationRule(x => x.Quantity, static value => value > 0, static value => $"Choose more than {value} items.");
        CheckDeliveryError(form, MissingName);

        form.Name = "Ada";
        if (!name.IsValid || quantity.IsValid || SingleLineFormatter.Default.Format(quantity.Message) != "Choose more than 0 items.")
        {
            throw new InvalidOperationException("Only the quantity should need a correction.");
        }

        form.Quantity = 1;
        if (!form.ValidationContext.GetIsValid() || form.HasErrors)
        {
            throw new InvalidOperationException("The completed checkout should be valid.");
        }

        form.ClearValidationRules(x => x.Quantity);
        form.Quantity = 0;
        if (!form.ValidationContext.IsValid || form.ValidationContext.Validations.Count != 1)
        {
            throw new InvalidOperationException("Only the delivery name rule should remain.");
        }
    }

    /// <summary>Looks up the message to display beside the delivery name field.</summary>
    /// <param name="form">The checkout being edited.</param>
    /// <param name="message">The expected field error.</param>
    /// <exception cref="InvalidOperationException">The field has no matching error.</exception>
    private static void CheckDeliveryError(CheckoutForm form, string message)
    {
        foreach (string error in form.GetErrors(nameof(CheckoutForm.Name)))
        {
            if (form.HasErrors && error == message)
            {
                return;
            }
        }

        throw new InvalidOperationException("The form should report the missing name on the delivery field.");
    }
}
