/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Eto.Drawing;
using log4net;

namespace HVH.Client
{
    /// <summary>
    /// Class that contains small helper functions
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Starts a new thread
        /// </summary>
        public static Thread StartThread(ThreadStart start, Boolean background)
        {
            Thread t = new Thread(start);
            t.IsBackground = background;
            t.Start();
            log.DebugFormat("Started new thread: {0}", start.Method.Name);
            return t;
        }

        /// <summary>
        /// Loads a bitmap from a relative path
        /// </summary>
        public static Bitmap LoadBitmap(String path)
        {
            Byte[] buffer = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), path));
            return new Bitmap(buffer);
        }
    }
}