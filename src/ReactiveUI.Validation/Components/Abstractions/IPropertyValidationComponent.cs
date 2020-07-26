// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Validation.Components.Abstractions
{
    /// <summary>
    /// a component specifically validating one or more properties.
    /// </summary>
    /// <typeparam name="TViewModel">The validation target.</typeparam>
    public interface IPropertyValidationComponent<TViewModel> : IValidationComponent, IValidatesProperties<TViewModel>
    {
    }
}
