// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Components.Abstractions;

/// <summary>
/// A component specifically validating one or more untyped properties.
/// </summary>
public interface IPropertyValidationComponent : IValidationComponent, IValidatesProperties
{
}