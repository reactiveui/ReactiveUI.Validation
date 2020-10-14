// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Validation.Components.Abstractions
{
    /// <summary>
    /// A component specifically validating one or more typed properties.
    /// </summary>
    /// <typeparam name="TViewModel">The validation target.</typeparam>
    [Obsolete("Consider using the non-generic version of an IPropertyValidationComponent.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Same type just generic.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same type just generic.")]
    public interface IPropertyValidationComponent<TViewModel> : IPropertyValidationComponent, IValidatesProperties<TViewModel>
    {
    }

    /// <summary>
    /// A component specifically validating one or more untyped properties.
    /// </summary>
    public interface IPropertyValidationComponent : IValidationComponent, IValidatesProperties
    {
    }
}
