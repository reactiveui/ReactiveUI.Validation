// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.Components.Abstractions;
#else
namespace ReactiveUI.Validation.Components.Abstractions;
#endif

/// <summary>A component specifically validating one or more untyped properties.</summary>
public interface IPropertyValidationComponent : IValidationComponent, IValidatesProperties;
