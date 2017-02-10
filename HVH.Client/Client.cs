/**
 * HVH.Client - User interface for the HVH.* infrastructure
 * Copyright (c) Dorian Stoll 2017
 * Licensed under the terms of the MIT License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Helios.Exceptions;
using Helios.Net;
using Helios.Topology;
using HVH.Common.Connection;
using HVH.Common.Encryption;
using HVH.Common.Interfaces;
using HVH.Common.Plugins;
using HVH.Common.Settings;
using log4net;

namespace HVH.Client
{
    /// <summary>
    /// Contains all functions for interacting with the server
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// The active instance of the client
        /// </summary>
        public static Client Instance { get; set; }

        /// <summary>
        /// The connection wrapper for communicating with the server
        /// </summary>
        public ConnectionWorker Connection { get; set; }

        /// <summary>
        /// A component that handles encryption of our messages
        /// </summary>
        public IEncryptionProvider encryption { get; set; }

        /// <summary>
        /// Whether we could log into the server
        /// </summary>
        public Boolean SessionCreated { get; set; }

        /// <summary>
        /// The threads created by the application
        /// </summary>
        public List<Thread> Threads { get; set; }        
        
        // The last message received from the server
        private List<String> messageBacklog = new List<String>();     
        
        // Whether we sent data to the server, waiting for an answer
        private Boolean sessionDataPending;

        /// <summary>
        /// Creates a new Connection to the server
        /// </summary>
        public Client()
        {
            // Connecting to the server
            Connection = new ConnectionWorker(ConnectionSettings.Instance.server, ConnectionSettings.Instance.port);
            Connection.Established = ConnectionEstablished;
            Connection.Received = DataReceived;
            Connection.Terminated = ConnectionTerminated;
            Threads = new List<Thread>();
        }
        
        /// <summary>
        /// Handles the login procedure for the Server
        /// </summary>
        /// <param name="node"></param>
        /// <param name="connection"></param>
        private void ConnectionEstablished(INode node, IConnection connection)
        {
            log.Info("Connection established. Sending public RSA key.");
            RSAEncryptionProvider rsa = new RSAEncryptionProvider(SecuritySettings.Instance.keySize);
            encryption = rsa;
            connection.Send(Communication.CLIENT_SEND_PUBLIC_KEY, node, new NoneEncryptionProvider());
            connection.Send(rsa.key.ToXmlString(false), node, new NoneEncryptionProvider());
            connection.BeginReceive();
        }

        /// <summary>
        /// Handles new messages by the server
        /// </summary>
        /// <param name="networkData"></param>
        /// <param name="connection"></param>
        private void DataReceived(NetworkData networkData, IConnection connection)
        {
            Byte[] buffer = networkData.Buffer;
            buffer = encryption.Decrypt(buffer);
            log.DebugFormat("Message received. Length: {0}", buffer.Length);

            String message = Encoding.UTF8.GetString(buffer);
            if (message != Communication.SERVER_SEND_WAIT_SIGNAL)
                messageBacklog.Add(message);

            // Do we have a message cached
            if (buffer.Length == 32 && messageBacklog.Count == 1)
            {
                // Handle messages who dont have additional parameters  
                if (!SessionCreated && sessionDataPending &&
                    message != Communication.SERVER_SEND_SESSION_CREATED)
                {
                    // Invalid connection
                    log.Fatal("Server is talking an invalid connection protocol!");
                    connection.Send(Communication.CLIENT_SEND_DISCONNECT, networkData.RemoteHost, encryption);
                    Connection.Client.Close();
                    Environment.Exit(1);
                }
                else if (message == Communication.SERVER_SEND_HEARTBEAT_CHALLENGE)
                {
                    log.Debug("Heartbeat received");
                    connection.Send(Communication.CLIENT_SEND_HEARTBEAT, networkData.RemoteHost, encryption);
                    connection.Send(Environment.UserName, networkData.RemoteHost, encryption);
                    messageBacklog.Clear();
                }
                else if (message == Communication.SERVER_SEND_DISCONNECT)
                {
                    // Server has gone offline, go and die too
                    log.Fatal("Server went offline.");
                    connection.Send(Communication.CLIENT_SEND_DISCONNECT, networkData.RemoteHost, encryption);
                    Connection.Client.Close();
                    Environment.Exit(1);
                }
            }
            else if (messageBacklog.Count > 1)
            {
                if (messageBacklog[0] == Communication.SERVER_SEND_SESSION_KEY)
                {
                    // Load the Encoder Type
                    try
                    {
                        Type encoderType = PluginManager.GetType<IEncryptionProvider>(SecuritySettings.Instance.encryption);
                        encryption = (IEncryptionProvider)Activator.CreateInstance(encoderType);
                    }
                    catch (Exception e)
                    {
                        log.Error("Invalid Encryption Provider! Falling back to no encryption", e);

                        // Fallback to None
                        encryption = new NoneEncryptionProvider();
                    }

                    // Log
                    log.InfoFormat("Received session key. Used encryption: {0}", encryption.GetType().Name);

                    // Apply session key
                    encryption.ChangeKey(buffer);

                    // Send session Data
                    log.Info("Sending session data");
                    connection.Send(Communication.CLIENT_SEND_USERDATA, networkData.RemoteHost, encryption);
                    connection.Send(Environment.MachineName, networkData.RemoteHost, encryption);
                    connection.Send(Environment.UserName, networkData.RemoteHost, encryption);
                    connection.Send(Communication.CLIENT_ID, networkData.RemoteHost, encryption);
                    log.Info("Sucessfully send session data");
                    sessionDataPending = true;

                    messageBacklog.Clear();
                }
                else if (!SessionCreated && sessionDataPending && messageBacklog[0] == Communication.SERVER_SEND_SESSION_CREATED)
                {
                    if (message == Communication.SERVER_ID)
                    {
                        sessionDataPending = false;
                        SessionCreated = true;
                        log.Info("Session created");
                    }
                    else
                    {
                        // Invalid connection
                        log.Info("Invalid connection");
                        connection.Send(Communication.CLIENT_SEND_DISCONNECT, networkData.RemoteHost, encryption);
                        Connection.Client.Close();
                        Environment.Exit(1);
                    }

                    // Clear
                    messageBacklog.Clear();
                }
            }
        }

        /// <summary>
        /// Handles an abrupt termination of the connection
        /// </summary>
        /// <param name="heliosConnectionException"></param>
        /// <param name="connection"></param>
        private void ConnectionTerminated(HeliosConnectionException heliosConnectionException, IConnection connection)
        {
            // Server has gone offline, go and die too
            log.FatalFormat("Server went offline. Reason: {0}", heliosConnectionException.Message);
            Connection.Client.Close();
            Environment.Exit(1);
        }
    }
}