/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Eto.Drawing;
using Helios.Net;
using Helios.Topology;
using HVH.Common.Interfaces;
using log4net;

namespace HVH.Client
{
    /// <summary>
    /// Class that contains small helper functions
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Lock object for sending operations
        /// </summary>
        private static Object sendLock = new Object();

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
        /// Abstraction layer over connection.Send()
        /// </summary>
        public static void Send(this IConnection connection, String data, INode node, IEncryptionProvider encryption = null)
        {
            lock (sendLock)
            {
                Byte[] buffer = Encoding.UTF8.GetBytes(data);
                if (encryption != null)
                {
                    buffer = encryption.Encrypt(buffer);
                }
                log.DebugFormat("Message sent. Length: {0}", buffer.Length);
                connection.Send(buffer, 0, buffer.Length, node);
            }
        }

        /// <summary>
        /// Abstraction layer over connection.Send()
        /// </summary>
        public static void Send(this IConnection connection, String[] data, INode node, IEncryptionProvider encryption = null)
        {
            lock (sendLock)
            {
                foreach (String s in data)
                {
                    Byte[] buffer = Encoding.UTF8.GetBytes(s);
                    if (encryption != null)
                    {
                        buffer = encryption.Encrypt(buffer);
                    }
                    log.DebugFormat("Message sent. Length: {0}", buffer.Length);
                    connection.Send(buffer, 0, buffer.Length, node);
                }
            }
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