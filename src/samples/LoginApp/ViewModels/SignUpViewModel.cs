// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to the ReactiveUI and Contributors under one or more agreements.
// The ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LoginApp.Services;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using ReactiveUI.Validation.States;
using Splat;

namespace LoginApp.ViewModels;

/// <summary>
/// A view model which shows controls to create an account.
/// This class demonstrates various ways to use ReactiveUI.Validation for property-level
/// and asynchronous validation.
/// Inheriting from <see cref="ReactiveValidationObject"/> provides a <see cref="ValidationContext"/>
/// that manages all validation rules for this instance.
/// </summary>
public partial class SignUpViewModel : ReactiveValidationObject, IRoutableViewModel, IActivatableViewModel
{
    private readonly ObservableAsPropertyHelper<bool> _isBusy;
    private readonly IUserDialogs? _dialogs;
    private readonly CompositeDisposable _disposables = [];

    /// <summary>
    /// Gets or sets the typed <see cref="UserName"/>.
    /// This property is validated both synchronously (required) and asynchronously (availability check).
    /// </summary>
    [Reactive]
    private string _userName = string.Empty;

    /// <summary>
    /// Gets or sets the typed <see cref="Password"/>.
    /// This property has multiple validation rules: required and minimum length.
    /// </summary>
    [Reactive]
    private string _password = string.Empty;

    /// <summary>
    /// Gets or sets the typed <see cref="ConfirmPassword"/>.
    /// This property is validated against the <see cref="Password"/> property.
    /// </summary>
    [Reactive]
    private string _confirmPassword = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignUpViewModel"/> class.
    /// </summary>
    /// <param name="hostScreen">The screen used for routing purposes.</param>
    /// <param name="dialogs"><see cref="IUserDialogs"/> implementation to show dialogs.</param>
    public SignUpViewModel(IScreen? hostScreen = null, IUserDialogs? dialogs = null)
    {
        _dialogs = dialogs ?? Locator.Current.GetService<IUserDialogs>();
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

        // The SignUp command is only executable when the ViewModel is valid.
        // this.IsValid() returns an IObservable<bool> indicating the validation state.
        SignUp = ReactiveCommand.Create(SignUpImpl, this.IsValid());

        // 1. Basic property validation rule.
        // Validates that UserName is not empty.
        this.ValidationRule(
            vm => vm.UserName,
            name => !string.IsNullOrWhiteSpace(name),
            "UserName is required.")
            .DisposeWith(_disposables);

        // 2. Multiple rules for a single property.
        // First rule: Password is required.
        this.ValidationRule(
            vm => vm.Password,
            password => !string.IsNullOrWhiteSpace(password),
            "Password is required.")
            .DisposeWith(_disposables);

        // Second rule: Password length requirement.
        // Demonstrates using a lambda for the error message to include dynamic information.
        this.ValidationRule(
            vm => vm.Password,
            password => password?.Length > 2,
            password => $"Password should be longer, current length: {password!.Length}")
            .DisposeWith(_disposables);

        // 3. Simple cross-property validation.
        this.ValidationRule(
            vm => vm.ConfirmPassword,
            confirmation => !string.IsNullOrWhiteSpace(confirmation),
            "Confirm password field is required.")
            .DisposeWith(_disposables);

        // 4. Complex validation rule using an IObservable.
        // Here we construct an IObservable<bool> that defines a complex validation rule
        // based on multiple properties. We associate this IObservable<bool> with the
        // 'ConfirmPassword' property via a call to the ValidationRule extension method.
        var passwordsObservable =
            this.WhenAnyValue(
                x => x.Password,
                x => x.ConfirmPassword,
                (password, confirmation) =>
                    password == confirmation);

        this.ValidationRule(
            vm => vm.ConfirmPassword,
            passwordsObservable,
            "Passwords must match.")
            .DisposeWith(_disposables);

        // 5. Asynchronous validation rule.
        // Here we pass a complex IObservable<IValidationState> to the ValidationRule.
        // That observable emits an empty state when UserName is valid, and emits an
        // error state when UserName is either invalid, or just changed and hasn't been validated yet.

        // Asynchronous validation logic with throttling to avoid excessive calls.
        var usernameValidated =
            this.WhenAnyValue(x => x.UserName)
                .Throttle(TimeSpan.FromSeconds(0.7), RxSchedulers.TaskpoolScheduler)
                .SelectMany(ValidateNameImpl)
                .ObserveOn(RxSchedulers.MainThreadScheduler);

        // State to show while validation is in progress.
        var usernameDirty =
            this.WhenAnyValue(x => x.UserName)
                .Select(_ => new ValidationState(false, "Please wait..."));

        // Merge both states: "Please wait..." immediately, followed by the actual result.
        this.ValidationRule(
            vm => vm.UserName,
            usernameValidated.Merge(usernameDirty));

        // Use the validation state to drive a 'Busy' indicator.
        _isBusy = usernameValidated
            .Select(_ => false)
            .Merge(usernameDirty.Select(_ => true))
            .ToProperty(this, x => x.IsBusy)
            .DisposeWith(_disposables);
    }

    /// <summary>
    /// Gets a value indicating whether the form is currently validating asynchronously.
    /// </summary>
    public bool IsBusy => _isBusy.Value;

    /// <summary>
    /// Gets a command which will create the account.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SignUp { get; }

    /// <summary>
    /// Gets the current page path.
    /// </summary>
    public string UrlPathSegment { get; } = "Sign Up";

    /// <summary>
    /// Gets the screen used for routing operations.
    /// </summary>
    public IScreen HostScreen { get; }

    /// <summary>
    /// Gets the activator which contains context information for use in activation of the view model.
    /// </summary>
    public ViewModelActivator Activator { get; } = new();

    /// <summary>
    /// Disposes the specified disposing.
    /// </summary>
    /// <param name="disposing">if set to <c>true</c> [disposing].</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposables.Dispose();
            _isBusy.Dispose();
        }

        base.Dispose(disposing);
    }

    private static async Task<IValidationState> ValidateNameImpl(string username)
    {
        await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
        return username.Length < 2
            ? new ValidationState(false, "The name is too short.")
            : username.Any(letter => !char.IsLetter(letter))
                ? new ValidationState(false, "Only letters allowed.")
                : ValidationState.Valid;
    }

    private void SignUpImpl() => _dialogs!.ShowDialog("User created successfully.");
}
