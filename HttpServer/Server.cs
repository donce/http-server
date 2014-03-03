using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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
