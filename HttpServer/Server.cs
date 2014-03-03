using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HttpServer
{
    class Server
    {
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(8080);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                HttpService service = new HttpService(client);
                Action action = new Action(service.Process);
                Task.Run(action);
            }
        }

    }
}
