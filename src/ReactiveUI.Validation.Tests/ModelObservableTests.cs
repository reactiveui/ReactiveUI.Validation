// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Validation.Components;
using ReactiveUI.Validation.Tests.Models;
using Xunit;

namespace ReactiveUI.Validation.Tests
{
    /// <summary>
    /// Tests for <see cref="ModelObservableValidation{TViewModel}"/>.
    /// </summary>
    public class ModelObservableTests
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

            var validation = new ModelObservableValidation<TestViewModel>(
                _validModel,
                _ => _validState,
                (text, valid) => "broken");

            Assert.True(validation.IsValid);
        }

        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateOfPropertyValidationIsCorrectTest()
        {
            _validState.OnNext(true);

            var propertyValidation = new ModelObservableValidation<TestViewModel, string>(
                _validModel,
                state => state.Name,
                _ => _validState,
                (text, valid) => "broken");

            Assert.True(propertyValidation.IsValid);
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidTest()
        {
            var validation = new ModelObservableValidation<TestViewModel>(
                _validModel,
                _ => _validState,
                (text, valid) => "broken");

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
            var propertyValidation = new ModelObservableValidation<TestViewModel, string>(
                _validModel,
                state => state.Name,
                _ => _validState,
                (text, valid) => "broken");

            _validState.OnNext(false);
            _validState.OnNext(true);
            _validState.OnNext(false);

            Assert.False(propertyValidation.IsValid);
            Assert.Equal("broken", propertyValidation.Text?.ToSingleLine());
        }

        /// <summary>
        /// Verifies that a call to Dispose disconnects the underlying observable
        /// of a <see cref="ModelObservableValidation{TViewModel}"/>.
        /// </summary>
        [Fact]
        public void DisposeShouldStopTrackingTheObservable()
        {
            var validation = new ModelObservableValidation<TestViewModel>(
                _validModel,
                _ => _validState,
                (text, valid) => "broken");

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
        /// of a <see cref="ModelObservableValidation{TViewModel,TViewModelProp}"/>.
        /// </summary>
        [Fact]
        public void DisposeShouldStopTrackingThePropertyValidationObservable()
        {
            var validation = new ModelObservableValidation<TestViewModel, string>(
                _validModel,
                state => state.Name,
                _ => _validState,
                (text, valid) => "broken");

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
    }
}
