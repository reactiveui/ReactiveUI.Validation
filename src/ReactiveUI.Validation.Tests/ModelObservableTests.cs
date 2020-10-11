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
        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateIsCorrectTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };
            var validState = new Subject<bool>();

            var v = new ModelObservableValidation<TestViewModel>(
                model,
                _ => validState.StartWith(true),
                (text, valid) => "broken");

            Assert.True(v.IsValid);
        }

        /// <summary>
        /// Verifies if the initial state is True.
        /// </summary>
        [Fact]
        public void InitialValidStateOfPropertyValidationIsCorrectTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };
            var validState = new Subject<bool>();

            var v = new ModelObservableValidation<TestViewModel, string>(
                model,
                state => state.Name,
                _ => validState.StartWith(true),
                (text, valid) => "broken");

            Assert.True(v.IsValid);
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };
            var validState = new ReplaySubject<bool>(1);

            var v = new ModelObservableValidation<TestViewModel>(
                model,
                _ => validState,
                (text, valid) => "broken");

            validState.OnNext(false);
            validState.OnNext(true);
            validState.OnNext(false);

            Assert.False(v.IsValid);
            Assert.Equal("broken", v.Text.ToSingleLine());
        }

        /// <summary>
        /// Verifies if the observable returns invalid.
        /// </summary>
        [Fact]
        public void ObservableToInvalidOfPropertyValidationTest()
        {
            var model = new TestViewModel { Name = "name", Name2 = "name2" };
            var validState = new ReplaySubject<bool>(1);

            var v = new ModelObservableValidation<TestViewModel, string>(
                model,
                state => state.Name,
                _ => validState,
                (text, valid) => "broken");

            validState.OnNext(false);
            validState.OnNext(true);
            validState.OnNext(false);

            Assert.False(v.IsValid);
            Assert.Equal("broken", v.Text.ToSingleLine());
        }
    }
}
