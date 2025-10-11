// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using DynamicData;
using ReactiveUI.Validation.Components.Abstractions;

namespace ReactiveUI.Validation.Contexts;

/// <inheritdoc cref="IReactiveObject" />
/// <inheritdoc cref="ICancelable" />
/// <inheritdoc cref="IValidationComponent" />
/// <summary>
/// The overall context for a view model under which validation takes place.
/// </summary>
/// <remarks>
/// Contains all of the <see cref="IValidationComponent" /> instances
/// applicable to the view model.
/// </remarks>
public interface IValidationContext : IValidationComponent, IReactiveObject, ICancelable
{
    /// <summary>
    /// Gets an observable for the Valid state.
    /// </summary>
    IObservable<bool> Valid { get; }

    /// <summary>
    /// Gets get the list of validations.
    /// </summary>
    IObservableList<IValidationComponent> Validations { get; }

    /// <summary>
    /// Adds a validation into the validations collection.
    /// </summary>
    /// <param name="validation">Validation component to be added into the collection.</param>
    void Add(IValidationComponent validation);

    /// <summary>
    /// Removes a validation from the validations collection.
    /// </summary>
    /// <param name="validation">Validation component to be removed from the collection.</param>
    void Remove(IValidationComponent validation);

    /// <summary>
    /// Removes many validation components from the validations collection.
    /// </summary>
    /// <param name="validations">Validation components to be removed from the collection.</param>
    void RemoveMany(IEnumerable<IValidationComponent> validations);

    /// <summary>
    /// Returns if the whole context is valid checking all the validations.
    /// </summary>
    /// <returns>Returns true if the <see cref="ValidationContext"/> is valid, otherwise false.</returns>
    bool GetIsValid();
}
