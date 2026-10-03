// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Tests.Models;
#else
namespace ReactiveUI.Validation.Tests.Models;
#endif

/// <summary>Sample abstract view model that implements <see cref="IReactiveObject" />.</summary>
public interface ISampleViewModel : IReactiveObject, IValidatableViewModel
{
    /// <summary>Gets or sets the name property used for testing.</summary>
    string Name { get; set; }
}
