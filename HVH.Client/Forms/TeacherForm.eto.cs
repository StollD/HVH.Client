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
using System.Linq;

namespace HVH.Client.Forms
{
    /// <summary>
    /// Renders the window with the control options for teachers (control computers in the local room)
    /// </summary>
    public partial class TeacherForm : Form
    {        
        // Associations to all controls
        private static Dictionary<String, Control> controls = new Dictionary<String, Control>();

        private void InitializeComponent()
        {
            Title = "HVH.Client";
            BackgroundColor = Colors.White;
            Icon = new Icon(Directory.GetCurrentDirectory() + "/assets/helmholtz_owl.ico");
            ClientSize = new Size(280, 430);
            Resizable = false;
            Minimizable = false;
            Maximizable = false;

            controls["roomL"] = new Label
            {
                Text = "Current Room:",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                Size = new Size(400, 20)
            };
            controls["room"] = new Label
            {
                Text = "ADMIN-RAUM",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                Size = new Size(400, 20)
            };
            controls["activeL"] = new Label
            {
                Text = "Active Clients:",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                Size = new Size(400, 20)
            };
            controls["active"] = new Label
            {
                Text = "2",
                Font = new Font("Segoe UI", 9),
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                Size = new Size(400, 20)
            };
            controls["seperator1"] = new ProgressBar
            {
                BackgroundColor = Colors.Black,
                Indeterminate = false,
                Value = 0,
                Size = new Size(260, 1)
            };
            controls["scroll"] = new Scrollable
            {
                Size = new Size(240, 200),
                Content = new StackLayout
                {
                    Orientation = Orientation.Vertical,
                    Items =
                    {
                        new StackLayoutItem { Control = new CheckBox { Font = new Font("Segoe UI", 10), Text = "LAPTOP-07", ThreeState = false } },
                        new StackLayoutItem { Control = new CheckBox { Font = new Font("Segoe UI", 10), Text = "LAPTOP-03", ThreeState = false } },
                        new StackLayoutItem { Control = new CheckBox { Font = new Font("Segoe UI", 10), Text = "ADMIN-PC9", ThreeState = false } }
                    },
                    Padding = new Padding(5),
                    Spacing = 5
                },
                //Border = BorderType.None
            };
            controls["shutdown"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/power-off.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Shutdown the selected computers"
            };
            controls["restart"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/refresh.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Restart the selected computers"
            };
            controls["seperator2"] = new ProgressBar
            {
                BackgroundColor = Colors.Black,
                Indeterminate = false,
                Value = 0,
                Size = new Size(1, 32)
            };
            controls["lock"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/lock.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Lock the selected computers"
            };
            controls["unlock"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/unlock-alt.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Unlock the selected computers"
            };
            controls["seperator3"] = new ProgressBar
            {
                BackgroundColor = Colors.Black,
                Indeterminate = false,
                Value = 0,
                Size = new Size(1, 32)
            };
            controls["report"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/exclamation-triangle.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Report an issue to the system administrators"
            };
            controls["leave"] = new Button
            {
                Text = "",
                Size = new Size(30, 30),
                Image = Utility.LoadBitmap("assets/fontawesome/black/png/32/sign-out.png"),
                ImagePosition = ButtonImagePosition.Left,
                ToolTip = "Go back to the main menu"
            };

            // Events
            (controls["shutdown"] as Button).Click += async delegate
            {
                await SendShutdown(false);
            };
            (controls["restart"] as Button).Click += async delegate
            {
                await SendShutdown(true);
            };
            (controls["lock"] as Button).Click += async delegate
            {
                await SendLock(false);
            };
            (controls["unlock"] as Button).Click += async delegate
            {
                await SendLock(true);
            };


            PixelLayout layout = new PixelLayout();
            layout.Add(controls["roomL"], 20, 10);
            layout.Add(controls["room"], 135, 10);
            layout.Add(controls["activeL"], 20, 35);
            layout.Add(controls["active"], 135, 35);
            layout.Add(controls["seperator1"], 10, 65);
            layout.Add(controls["scroll"], 20, 80);
            layout.Add(controls["shutdown"], 20, 290);
            layout.Add(controls["restart"], 55, 290);
            layout.Add(controls["seperator2"], 95, 290);
            layout.Add(controls["lock"], 105, 290);
            layout.Add(controls["unlock"], 140, 290);
            layout.Add(controls["seperator3"], 180, 290);
            layout.Add(controls["report"], 190, 290);
            layout.Add(controls["leave"], 225, 290);
            Content = layout;
        }

        private void RoomNameReceived(String[] names)
        {
            (controls["room"] as Label).Text = names.FirstOrDefault();
            Client.Instance.RequestClients(names.FirstOrDefault(), false);
        }

        private void ClientsReceived(String[] names, Boolean locked)
        {
            Scrollable scroll = controls["scroll"] as Scrollable;
            StackLayout stack = scroll.Content as StackLayout;
            if (!locked)
            {
                stack.Items.Clear();
                for (Int32 i = 0; i < names.Length; i++)
                {
                    stack.Items.Add(new StackLayoutItem { Control = new CheckBox { Font = new Font("Segoe UI", 10), Text = names[i], ThreeState = false } });
                }
                
                // Get locked clients
                Client.Instance.RequestClients((controls["room"] as Label).Text, true);
            } 
            else
            {
                for (Int32 i = 0; i < stack.Items.Count; i++)
                {
                    if (names.Contains((stack.Items[i].Control as CheckBox).Text))
                    {
                        (stack.Items[i].Control as CheckBox).Text += " (locked)";
                    }
                }
            }
        }

        private Task SendShutdown(Boolean restart)
        {
            Scrollable scroll = controls["scroll"] as Scrollable;
            StackLayout stack = scroll.Content as StackLayout;
            List<String> clients = new List<String>();
            for (Int32 i = 0; i < stack.Items.Count; i++)
            {
                CheckBox box = stack.Items[i].Control as CheckBox;
                if (box.Checked.GetValueOrDefault())
                    clients.Add(box.Text);
            }
            return Client.Instance.SendShutdown(clients.ToArray(), restart);
        }

        private Task SendLock(Boolean unlock)
        {
            Scrollable scroll = controls["scroll"] as Scrollable;
            StackLayout stack = scroll.Content as StackLayout;
            List<String> clients = new List<String>();
            for (Int32 i = 0; i < stack.Items.Count; i++)
            {
                CheckBox box = stack.Items[i].Control as CheckBox;
                if (box.Checked.GetValueOrDefault())
                    clients.Add(box.Text);
            }
            return Client.Instance.SendLock(clients.ToArray(), unlock);
        }
    }
}
