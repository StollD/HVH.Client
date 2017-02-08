/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using Eto.Forms;
using Eto.Drawing;
using System;
using System.Threading;

namespace HVH.Client.Forms
{
    /// <summary>
    /// This class is responsible for initializing the application and connecting to the Server
    /// </summary>
    public partial class LoadingForm : Form
    {
        // The currently displayed Status
        private static Label status;

        private void InitializeComponent()
        {
            Title = "HVH.Client";
            ClientSize = new Size(400, 300);
            Resizable = false;
            Minimizable = false;
            Maximizable = false;
            Shown += OnLoadComplete;

            // Loading State
            status = new Label
            {
                Text = "HVH.Client - Version 0.0.0.1",
                Font = new Font(FontFamilies.Sans, 10),
                BackgroundColor = Colors.LightGrey,
                TextColor = Colors.DarkSlateGray,
                Size = new Size(400, 16),
                TextAlignment = TextAlignment.Center
            };

            Content = new DynamicLayout
            {
                Rows =
                {
                    new DynamicRow { new ImageView { Image = Utility.LoadBitmap("assets/helmholtzGymCartoon.png"), Size = new Size(400, -1) } },
                    new DynamicRow { status },
                    new DynamicRow { new ProgressBar { Indeterminate = true, Size = new Size(400, 25) } }
                }
            };
        }

        /// <summary>
        /// Updates the text of the status display using a fade-out / fade-in design
        /// </summary>
        public static void SetStatus(String text)
        {
            status.Text = text;
        }
    }
}