using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using System.IO;

namespace HVH.Client.Forms
{
    /// <summary>
    /// Renders a window with a list of areas the user can access
    /// </summary>
    public partial class MenuForm : Form
    {        
        // Associations to all controls
        private static Dictionary<String, Control> controls = new Dictionary<String, Control>();

        void InitializeComponent()
        {
            Title = "HVH.Client";
            BackgroundColor = Colors.White;
            Icon = new Icon(Directory.GetCurrentDirectory() + "/assets/helmholtz_owl.ico");
            ClientSize = new Size(280, 430);
            Resizable = false;
            Minimizable = false;
            Maximizable = false;

            controls["logo"] = new ImageView { Image = Utility.LoadBitmap("assets/helmholtz_owl.png"), Size = new Size(200, -1) };
            controls["roomControl"] = new Button { Text = "Room Control",Size = new Size(240, 30), Enabled = Client.Instance?.Status.Type != UserType.Normal };
            controls["issueReport"] = new Button { Text = "Issue Reporting", Size = new Size(240, 30), Enabled = Client.Instance?.Status.Type != UserType.Normal  };
            controls["adminPanel"] = new Button { Text = "Admin Panel", Size = new Size(240, 30), Enabled = Client.Instance?.Status.Type == UserType.Admin };
            controls["seperator"] = new ProgressBar
            {
                BackgroundColor = Colors.Black,
                Indeterminate = false,
                Value = 0,
                Size = new Size(240, 1)
            };
            controls["logout"] = new Button { Text = "Logout", Size = new Size(240, 30) };

            // Events
            (controls["logout"] as Button).Click += delegate
            {
                Client.Instance.SendLogout();
            };
            (controls["roomControl"] as Button).Click += delegate
            {
                Application.Instance.MainForm = new RoomControlForm();
                Application.Instance.MainForm.Show();
                Visible = false;
            };
            PixelLayout layout = new PixelLayout();
            layout.Add(controls["logo"], 40, 10);
            layout.Add(controls["roomControl"], 20, 250);
            layout.Add(controls["issueReport"], 20, 290);
            layout.Add(controls["adminPanel"], 20, 330);
            layout.Add(controls["seperator"], 20, 370);
            layout.Add(controls["logout"], 20, 380);
            Content = layout;
        }
    }
}
