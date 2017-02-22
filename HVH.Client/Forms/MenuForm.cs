/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HVH.Common.Plugins;
using log4net;
using log4net.Config;

namespace HVH.Client.Forms
{
    /// <summary>
    /// Renders a window with a list of areas the user can access
    /// </summary>
    public partial class MenuForm
    {
        public MenuForm()
        {
            InitializeComponent();
        }
    }
}
