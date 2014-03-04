using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace HttpServer
{
    public class ServerClass
    {
        private int _port;
        private bool _accepting = true;

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Thread _mainThread;

        private List<Task> tasks = new List<Task>();

        public ServerClass(int port)
        {
            _port = port;
        }

        public void Start()
        {
            Configuration config = new Configuration("../../../configuration.xml");
            Console.WriteLine(config.RootPath);

            _mainThread = Thread.CurrentThread;
            log4net.Config.XmlConfigurator.Configure();

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Stop();
            };

            TcpListener listener = new TcpListener(_port);
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
