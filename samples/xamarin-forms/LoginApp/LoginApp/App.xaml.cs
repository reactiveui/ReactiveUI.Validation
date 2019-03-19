// <copyright file="ReactiveUI.Validation/samples/xamarin-forms/LoginApp/LoginApp/App.xaml.cs" company=".NET Foundation">
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// </copyright>

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace LoginApp
{
    /// <inheritdoc />
    /// <summary>
    /// The main application instance.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();

            var unused = new AppBootstrapper();
            MainPage = AppBootstrapper.CreateMainPage();
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            // Handle when your app starts
        }

        /// <inheritdoc/>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <inheritdoc/>
        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}