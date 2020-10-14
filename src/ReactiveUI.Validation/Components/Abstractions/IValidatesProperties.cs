// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ReactiveUI.Validation.Components.Abstractions
{
    /*
    /// <summary>
    /// Interface marking a validation component that validates specific typed properties.
    /// </summary>
    /// <typeparam name="TViewModel">The validation target.</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Same type just generic.")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Same type just generic.")]
    public interface IValidatesProperties<TViewModel> : IValidatesProperties
    {
        /// <summary>
        /// Determine if a property name is actually contained within this.
        /// </summary>
        /// <typeparam name="TProp">Any type.</typeparam>
        /// <param name="propertyExpression">ViewModel property.</param>
        /// <param name="exclusively">Indicates if the property to find is unique.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        bool ContainsProperty<TProp>(Expression<Func<TViewModel, TProp>> propertyExpression, bool exclusively = false);
    }
    */

    /// <summary>
    /// Interface marking a validation component that validates specific untyped properties.
    /// </summary>
    public interface IValidatesProperties
    {
        /// <summary>
        /// Gets the total number of properties referenced.
        /// </summary>
        int PropertyCount { get; }

        /// <summary>
        /// Gets the properties associated with this validation component.
        /// </summary>
        IEnumerable<string> Properties { get; }

        /// <summary>
        /// Determine if a property name is actually contained within this.
        /// </summary>
        /// <param name="propertyName">ViewModel property name.</param>
        /// <param name="exclusively">Indicates if the property to find is unique.</param>
        /// <returns>Returns true if it contains the property, otherwise false.</returns>
        bool ContainsPropertyName(string propertyName, bool exclusively = false);
    }
}
