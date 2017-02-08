﻿/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

 using System;
using Eto;
using Eto.Forms;

namespace HVH.Client.Linux
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Platforms.Gtk3).Run(new LoadingForm());
        }
    }
}
