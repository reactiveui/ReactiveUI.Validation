// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Components.Abstractions;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for the generic <see cref="ObservableValidation{TViewModel, TValue}"/> and for
    /// <see cref="ObservableValidation{TViewModel,TValue,TProp}"/> as well.
    /// </summary>
    public class ObservableValidationTests
    {
        private readonly ISubject<bool> _validState = new ReplaySubject<bool>(1);
        private readonly TestViewModel _validModel = new TestViewModel
        {
            Name = "name",
            Name2 = "name2"
        };

        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateIsCorrectTest()
        {
            _validState.OnNext(true);

            var validation = new ObservableValidation<TestViewModel, bool>(
                _validModel,
                _validState,
                valid => valid,
                "broken");

            Assert.True(validation.IsValid);
        }

        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateOfPropertyValidationIsCorrectTest()
        {
            _validState.OnNext(true);

            var propertyValidation = new ObservableValidation<TestViewModel, bool, string>(
                _validModel,
                state => state.Name,
                _validState,
                valid => valid,
                "broken");

            Assert.True(propertyValidation.IsValid);
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidTest()
        {
            var validation = new ObservableValidation<TestViewModel, bool>(
                _validModel,
                _validState,
                valid => valid,
                "broken");

            _validState.OnNext(false);
            _validState.OnNext(true);
            _validState.OnNext(false);

            Assert.False(validation.IsValid);
            Assert.Equal("broken", validation.Text?.ToSingleLine());
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidOfPropertyValidationTest()
        {
            var propertyValidation = new ObservableValidation<TestViewModel, bool, string>(
                _validModel,
                state => state.Name,
                _validState,
                valid => valid,
                "broken");

            _validState.OnNext(false);
            _validState.OnNext(true);
            _validState.OnNext(false);

            Assert.False(propertyValidation.IsValid);
            Assert.Equal("broken", propertyValidation.Text?.ToSingleLine());
        }

        /// <summary>
        /// Verifies that a call to Dispose disconnects the underlying observable
        /// of a <see cref="ObservableValidation{TViewModel,TValue}"/>.
        /// </summary>
        [Fact]
        public void DisposeShouldStopTrackingTheObservable()
        {
            var validation = new ObservableValidation<TestViewModel, bool>(
                _validModel,
                _validState,
                validity => validity,
                "broken");

            _validState.OnNext(true);

            Assert.True(validation.IsValid);

            _validState.OnNext(false);

            Assert.False(validation.IsValid);

            validation.Dispose();

            _validState.OnNext(true);
            _validState.OnNext(false);
            _validState.OnNext(true);

            Assert.False(validation.IsValid);
        }

        /// <summary>
        /// Verifies that a call to Dispose disconnects the underlying observable
        /// of a <see cref="ObservableValidation{TViewModel,TValue,TProp}"/>.
        /// </summary>
        [Fact]
        public void DisposeShouldStopTrackingThePropertyValidationObservable()
        {
            var validation = new ObservableValidation<TestViewModel, bool, string>(
                _validModel,
                state => state.Name,
                _validState,
                validity => validity,
                "broken");

            _validState.OnNext(true);

            Assert.True(validation.IsValid);

            _validState.OnNext(false);

            Assert.False(validation.IsValid);

            validation.Dispose();

            _validState.OnNext(true);
            _validState.OnNext(false);
            _validState.OnNext(true);

            Assert.False(validation.IsValid);
        }

        /// <summary>
        /// Verifies that we support resolving properties by expressions.
        /// </summary>
        [Fact]
        public void ShouldResolveTypedProperties()
        {
            var viewModel = new TestViewModel { Name = string.Empty };
            IPropertyValidationComponent component =
                new ObservableValidation<TestViewModel, string, string>(
                    viewModel,
                    model => model.Name,
                    viewModel.WhenAnyValue(x => x.Name),
                    state => !string.IsNullOrWhiteSpace(state),
                    "Name shouldn't be empty.");

            Assert.True(component.ContainsProperty<TestViewModel, string>(model => model.Name));
            Assert.True(component.ContainsProperty<TestViewModel, string>(model => model.Name, true));
            Assert.False(component.ContainsProperty<TestViewModel, string>(model => model.Name2));
            Assert.False(component.ContainsProperty<TestViewModel, string>(model => model.Name2, true));
            Assert.Throws<ArgumentNullException>(() => component.ContainsProperty<TestViewModel, string>(null!));
        }
    }
}
