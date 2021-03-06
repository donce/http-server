﻿using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace HttpServer
{
    /// <summary>
    /// The class to make the instance of the server
    /// </summary>
    public class ServerClass
    {
        private bool _accepting = true;

        public static Configuration Configuration;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Thread _mainThread;

        private List<Task> tasks = new List<Task>();

        /// <summary>
        /// The constructor of the server class
        /// </summary>
        /// <param name="configurationFilename">The name of the configuration file</param>
        public ServerClass(string configurationFilename)
        {
            Configuration = new Configuration(configurationFilename);
        }

        /// <summary>
        /// Start the server
        /// </summary>
        public void Start()
        {
            _mainThread = Thread.CurrentThread;
            log4net.Config.XmlConfigurator.Configure();

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Stop();
            };

            TcpListener listener = new TcpListener(Configuration.Port);
            listener.Start();
            log.Info("Server has started");

            try
            {
                while (_accepting)
                {
                    while (!listener.Pending())
                    {
                        Thread.Sleep(50);
                        if (!_accepting)
                            return;
                    }
                    TcpClient client = listener.AcceptTcpClient();
                    log.Info("New client accepted");
                    HttpService service = new HttpService(client);
                    Action action = new Action(service.Process);
                    log.Info("Sending reponse to client");
                    tasks.Add(Task.Run(action));
                    log.Info("Response sent");
                }
            }
            finally
            {
                listener.Stop();
                Task.WaitAll(tasks.ToArray());
                log.Warn("Server closing");
            }
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            if (_accepting)
            {
                Console.WriteLine("CLOSING");
                log.Warn("Server wants to close, waiting for pending connections");
                _accepting = false;
            }
        }
    }
}