// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Mocked ViewModel for INotifyDataErrorInfo testing.</summary>
[System.Diagnostics.DebuggerDisplay("IndeiTestViewModel: {Name}")]
public class IndeiTestViewModel : ReactiveValidationObject
{
    /// <summary>Initializes a new instance of the <see cref="IndeiTestViewModel"/> class.</summary>
    public IndeiTestViewModel()
        : base(ImmediateSequencer.Instance)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="IndeiTestViewModel"/> class.</summary>
    /// <param name="formatter">Validation text formatter.</param>
    public IndeiTestViewModel(IValidationTextFormatter<string> formatter)
        : base(ImmediateSequencer.Instance, formatter)
    {
    }

    /// <summary>Gets or sets the name.</summary>
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the other name used for cross-field validation testing.</summary>
    public string? OtherName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the minimum value for cross-field validation testing.</summary>
    public double MinValue
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the maximum value for cross-field validation testing.</summary>
    public double MaxValue
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
