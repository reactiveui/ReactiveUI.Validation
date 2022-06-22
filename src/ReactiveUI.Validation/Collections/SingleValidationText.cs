// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ReactiveUI.Validation.Collections;

internal sealed class SingleValidationText : IValidationText
{
    private readonly string _text;

    internal SingleValidationText(string text) => _text = text;

    public int Count => 1;

    public string this[int index] => index is 0 ? _text : throw new ArgumentOutOfRangeException(nameof(index));

    public IEnumerator<string> GetEnumerator()
    {
        yield return _text;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public string ToSingleLine(string? separator) => _text;
}
