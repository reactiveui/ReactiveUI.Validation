// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Validation.Reactive.ValidationBindings.Abstractions;
#else
namespace ReactiveUI.Validation.ValidationBindings.Abstractions;
#endif

/// <summary>A validation binding component.</summary>
public interface IValidationBinding : IDisposable;
