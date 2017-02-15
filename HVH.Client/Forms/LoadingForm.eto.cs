/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using Eto.Forms;
using Eto.Drawing;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace HVH.Client.Forms
{
    /// <summary>
    /// This class is responsible for initializing the application and connecting to the Server
    /// </summary>
    public partial class LoadingForm : Form
    {
        // Associations to all controls
        private static Dictionary<String, Control> controls = new Dictionary<String, Control>();

        private void InitializeComponent()
        {
            Title = "HVH.Client";
            Icon = new Icon(Directory.GetCurrentDirectory() + "/assets/helmholtz_owl.ico");
            ClientSize = new Size(400, 300);
            Resizable = false;
            Minimizable = false;
            Maximizable = false;
            Shown += OnLoadComplete;
            
            controls["status"] = new Label
            {
                Text = "HVH.Client - Version 0.0.0.1",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.LightGrey,
                TextColor = Colors.DarkSlateGray,
                Size = new Size(400, 16),
                TextAlignment = TextAlignment.Center
            };
            controls["background"] = new ImageView { Image = Utility.LoadBitmap("assets/helmholtzGymCartoon.png"), Size = new Size(400, 275) };
            controls["progress"] = new ProgressBar { Indeterminate = true, Size = new Size(400, 25) };

            PixelLayout layout = new PixelLayout();
            layout.Add(controls["background"], 0, 0);
            layout.Add(controls["progress"], 0, 291);
            layout.Add(controls["status"], 0, 275);
            Content = layout;
        }

        private void AssembleLoginForm()
        {
            // Deploy blends
            PixelLayout layout = Content as PixelLayout;
            controls["blend"] = new ImageView { Image = Utility.LoadBitmap("assets/blend.png"), Size = new Size(400, 275) };
            layout.Add(controls["blend"], 0, 0);
            Label status = controls["status"] as Label;
            status.Text = "Please enter your login credentials";
            ProgressBar progress = controls["progress"] as ProgressBar;
            progress.Indeterminate = false;
            progress.Value = progress.MinValue;

            // Add inputs
            controls["usernameL"] = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.WhiteSmoke,
                Size = new Size(250, 16),

            };
            controls["username"] = new TextBox
            {
                BackgroundColor = Colors.WhiteSmoke,
                Font = new Font("Segoe UI", 12),
                TextColor = Colors.DimGray,
                Size = new Size(250, 30)
            };
            controls["passwordL"] = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.WhiteSmoke,
                Size = new Size(250, 16),

            };
            controls["password"] = new PasswordBox
            {                
                BackgroundColor = Colors.WhiteSmoke,
                Font = new Font("Segoe UI", 12),
                PasswordChar = '*',
                TextColor = Colors.DimGray,
                Size = new Size(250, 30)                
            };
            controls["submit"] = new Button
            {
                Text = "Login",
                Font = new Font("Segoe UI", 12),
                Size = new Size(80, 30),
                BackgroundColor = Colors.WhiteSmoke,
                TextColor = Colors.DimGray,
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/angle-double-right.png"),
                ImagePosition = ButtonImagePosition.Left
            };
            (controls["submit"] as Button).Click += delegate (Object sender, EventArgs e)
            {
                Client.Instance.RegisterLoggedInAction(HandleLogin);
                Client.Instance.RegisterNoLoginAction(HandleNoLogin);
                Client.Instance.RegisterNoLoginServerAction(HandleNoLoginServer);
                try
                {
                    Client.Instance.Login((controls["username"] as TextBox).Text, (controls["password"] as PasswordBox).Text);
                    progress.Indeterminate = true;
                    status.Text = "Logging in...";
                }
                catch (Exception ex)
                {
                    ShowWarning(ex.Message, layout);
                    (controls["username"] as TextBox).Text = "";
                    (controls["password"] as PasswordBox).Text = "";
                    progress.Indeterminate = false;
                }
            };
            layout.Add(controls["usernameL"], 75, 50);
            layout.Add(controls["username"], 75, 70);
            layout.Add(controls["passwordL"], 75, 120);
            layout.Add(controls["password"], 75, 140);
            layout.Add(controls["submit"], 245, 190);
        }

        private void HandleLogin()
        {
            new TeacherForm().Show();
        }

        private void HandleNoLogin()
        {
            Label status = controls["status"] as Label;
            status.Text = "Please enter your login credentials";
            ProgressBar progress = controls["progress"] as ProgressBar;
            progress.Indeterminate = false;
            ShowWarning("Wrong username/password!", Content as PixelLayout);
            (controls["username"] as TextBox).Text = "";
            (controls["password"] as PasswordBox).Text = "";
        }

        private void HandleNoLoginServer()
        {
            Label status = controls["status"] as Label;
            status.Text = "Please enter your login credentials";
            ProgressBar progress = controls["progress"] as ProgressBar;
            progress.Indeterminate = false;
            ShowWarning("No login servers available!", Content as PixelLayout);
            (controls["username"] as TextBox).Text = "";
            (controls["password"] as PasswordBox).Text = "";
        }

        /// <summary>
        /// Updates the text of the status display using a fade-out / fade-in design
        /// </summary>
        public static async Task SetStatus(String text)
        {
            Label status = controls["status"] as Label;
            while (status.TextColor.Ab - 10 > -1)
            {
                status.TextColor = Color.FromArgb(status.TextColor.Rb, status.TextColor.Gb, status.TextColor.Bb, status.TextColor.Ab - 10);
                await Task.Delay(60);
            }
            status.Text = text;
            await Task.Delay(100);
            while (status.TextColor.Ab + 10 < 256)
            {
                status.TextColor = Color.FromArgb(status.TextColor.Rb, status.TextColor.Gb, status.TextColor.Bb, status.TextColor.Ab + 10);
                await Task.Delay(60);
            }
            
        }

        /// <summary>
        /// Displays a warning message
        /// </summary>
        public static void ShowWarning(String text, PixelLayout layout)
        {
            if (!controls.ContainsKey("warning"))
            {
                controls["warning"] = new Label
                {
                    Text = text,
                    Font = new Font("Segoe UI", 10),
                    BackgroundColor = Colors.Transparent,
                    TextColor = Colors.OrangeRed,
                    Size = new Size(160, 32),
                    Wrap = WrapMode.Word
                };
                layout.Add(controls["warning"], 75, 190);
            }
            (controls["warning"] as Label).Text = text;
        }
    }
}