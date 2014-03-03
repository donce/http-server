using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HttpServer
{
    class Server
    {
        public const int DefaultPort = 8080;
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(DefaultPort);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                HttpService service = new HttpService(client);
//                service.Process();
                Action action = new Action(service.Process);
                Task.Run(action);
            }
        }

    }
}
