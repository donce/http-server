using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace HttpServer
{
    public class ServerClass
    {
        private int _port;
        private static bool _accepting = true;

        ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Start(int port)
        {
            log4net.Config.XmlConfigurator.Configure();

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Stop();
            };

            TcpListener listener = new TcpListener(port);
            listener.Start();
            log.Info("Server has started");

            while (_accepting)
            {
                TcpClient client = listener.AcceptTcpClient();
                log.Info("New client accepted");
                HttpService service = new HttpService(client);
                Action action = new Action(service.Process);
                log.Info("Sending reponse to client");
                Task.Run(action);
                log.Info("Response sent");
            }
        }

        public void Stop()
        {
            Console.WriteLine("CLOSING");
            log.Warn("Server closing");
            _accepting = false;
        }
        //TODO:Stop method
    }
}
