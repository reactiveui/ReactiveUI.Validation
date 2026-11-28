// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Checkout;

/// <summary>Provides a delivery name and quantity for checkout.</summary>
internal sealed class CheckoutForm : ReactiveValidationObject
{
    /// <summary>Initializes a new instance of the <see cref="CheckoutForm"/> class.</summary>
    internal CheckoutForm()
        : base(Immediate.Instance, new SingleLineFormatter(" / "))
    {
    }

    /// <summary>Gets or sets the name on the delivery label.</summary>
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the number of items to deliver.</summary>
    public int Quantity
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
