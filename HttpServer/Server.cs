using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace HttpServer
{
    
    public class Server
    {
        public const int DefaultPort = 8080;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            TcpListener listener = new TcpListener(DefaultPort);
            listener.Start();
            log.Info("Server has started");

            while (true)
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
    }
}
