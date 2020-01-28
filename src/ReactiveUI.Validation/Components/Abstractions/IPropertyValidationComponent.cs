// <copyright file="ReactiveUI.Validation/src/ReactiveUI.Validation/Components/Abstractions/IValidationComponent.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

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
