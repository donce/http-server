using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using log4net;

namespace HttpServer
{
    
    public class Server
    {
        public const int DefaultPort = 8080;
        public static bool accepting = true;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
            {
                e.Cancel = true;
                Console.WriteLine("CLOSING");//TODO: log
                accepting = false;
            };

            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            TcpListener listener = new TcpListener(DefaultPort);
            listener.Start();
            log.Info("Server has started");


            //TODO: graceful
            while (accepting)
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
