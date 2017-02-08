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
using HVH.Common.Plugins;
using log4net;

namespace HVH.Client.Forms
{
    /// <summary>
    /// This class is responsible for initializing the application and connecting to the Server
    /// </summary>
    public partial class LoadingForm
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public LoadingForm()
        {
            // Create dirs
            Directory.CreateDirectory("logs/");
            Directory.CreateDirectory("plugins/");

            // Say hello
            log.Info("Client started.");
            log.Info("Creating Loading Form..");

            // Create the form
            InitializeComponent();
        }

        /// <summary>
        /// Fires when the form was successfully loaded.
        /// Here we establish a connection to the HVH.Server and prepare everything for login
        /// </summary>
        private void OnLoadComplete(Object sender, EventArgs e)
        {
            // Load plugins
            SetStatus("Loading Plugins...");
            PluginManager.LoadPlugins();

            // Create the client interface
            Client.Instance = new Client();
        }
    }
}
