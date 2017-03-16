﻿/**
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
    /// Renders the window with the control options for teachers (control computers in the local room)
    /// </summary>
    public partial class RoomControlForm
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RoomControlForm()
        {
            log.Info("Opening Teacher Room Control Window");
            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Here we fetch data from the HVH.Server
        /// </summary>
        private void LoadData()
        {
            Client.Instance.RegisterRoomNameReceivedAction(RoomNameReceived);
            Client.Instance.RegisterRoomComputersReceivedAction(ClientsReceived);
            Utility.StartThread(() =>
            {
                Client.Instance.FillRoomData();
                Thread.Sleep(5000);
            }, true);
        }
    }
}
