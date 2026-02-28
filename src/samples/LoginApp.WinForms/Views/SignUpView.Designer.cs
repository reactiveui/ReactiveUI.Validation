// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace LoginApp.WinForms.Views
{
    partial class SignUpView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UserNameTextBox = new System.Windows.Forms.TextBox();
            this.UserNameErrorLabel = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.PasswordErrorLabel = new System.Windows.Forms.Label();
            this.ConfirmPasswordTextBox = new System.Windows.Forms.TextBox();
            this.ConfirmPasswordErrorLabel = new System.Windows.Forms.Label();
            this.SignUpButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UserNameTextBox
            // 
            this.UserNameTextBox.Location = new System.Drawing.Point(34, 53);
            this.UserNameTextBox.Name = "UserNameTextBox";
            this.UserNameTextBox.PlaceholderText = "Please, enter the user name...";
            this.UserNameTextBox.Size = new System.Drawing.Size(235, 27);
            this.UserNameTextBox.TabIndex = 0;
            // 
            // UserNameErrorLabel
            // 
            this.UserNameErrorLabel.AutoSize = true;
            this.UserNameErrorLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.UserNameErrorLabel.Location = new System.Drawing.Point(29, 83);
            this.UserNameErrorLabel.Name = "UserNameErrorLabel";
            this.UserNameErrorLabel.Size = new System.Drawing.Size(152, 20);
            this.UserNameErrorLabel.TabIndex = 1;
            this.UserNameErrorLabel.Text = "User name error label";
            // 
            // PasswordTextBox
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(34, 107);
            this.PasswordTextBox.Name = "PasswordTextBox";
            this.PasswordTextBox.PlaceholderText = "Please, enter the password...";
            this.PasswordTextBox.Size = new System.Drawing.Size(235, 27);
            this.PasswordTextBox.TabIndex = 1;
            // 
            // PasswordErrorLabel
            // 
            this.PasswordErrorLabel.AutoSize = true;
            this.PasswordErrorLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.PasswordErrorLabel.Location = new System.Drawing.Point(29, 137);
            this.PasswordErrorLabel.Name = "PasswordErrorLabel";
            this.PasswordErrorLabel.Size = new System.Drawing.Size(143, 20);
            this.PasswordErrorLabel.TabIndex = 1;
            this.PasswordErrorLabel.Text = "Password error label";
            // 
            // ConfirmPasswordTextBox
            // 
            this.ConfirmPasswordTextBox.Location = new System.Drawing.Point(34, 160);
            this.ConfirmPasswordTextBox.Name = "ConfirmPasswordTextBox";
            this.ConfirmPasswordTextBox.PlaceholderText = "Please, confirm the password...";
            this.ConfirmPasswordTextBox.Size = new System.Drawing.Size(235, 27);
            this.ConfirmPasswordTextBox.TabIndex = 2;
            // 
            // ConfirmPasswordErrorLabel
            // 
            this.ConfirmPasswordErrorLabel.AutoSize = true;
            this.ConfirmPasswordErrorLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ConfirmPasswordErrorLabel.Location = new System.Drawing.Point(29, 190);
            this.ConfirmPasswordErrorLabel.Name = "ConfirmPasswordErrorLabel";
            this.ConfirmPasswordErrorLabel.Size = new System.Drawing.Size(202, 20);
            this.ConfirmPasswordErrorLabel.TabIndex = 1;
            this.ConfirmPasswordErrorLabel.Text = "Confirm password error label";
            // 
            // SignUpButton
            // 
            this.SignUpButton.Location = new System.Drawing.Point(34, 213);
            this.SignUpButton.Name = "SignUpButton";
            this.SignUpButton.Size = new System.Drawing.Size(235, 38);
            this.SignUpButton.TabIndex = 3;
            this.SignUpButton.Text = "Sign Up";
            this.SignUpButton.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(29, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Sign Up";
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.ErrorLabel.Location = new System.Drawing.Point(29, 254);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(240, 106);
            this.ErrorLabel.TabIndex = 1;
            this.ErrorLabel.Text = "View model error label";
            // 
            // SignUpView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(302, 369);
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SignUpButton);
            this.Controls.Add(this.ConfirmPasswordErrorLabel);
            this.Controls.Add(this.ConfirmPasswordTextBox);
            this.Controls.Add(this.PasswordErrorLabel);
            this.Controls.Add(this.PasswordTextBox);
            this.Controls.Add(this.UserNameErrorLabel);
            this.Controls.Add(this.UserNameTextBox);
            this.Name = "SignUpView";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UserNameTextBox;
        private System.Windows.Forms.TextBox PasswordTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ConfirmPasswordTextBox;
        private System.Windows.Forms.Button SignUpButton;
        private System.Windows.Forms.Label UserNameErrorLabel;
        private System.Windows.Forms.Label PasswordErrorLabel;
        private System.Windows.Forms.Label ConfirmPasswordErrorLabel;
        private System.Windows.Forms.Label ErrorLabel;
    }
}
