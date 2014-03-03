using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class HttpService
    {
        private TcpClient client;

        public HttpService(TcpClient client)
        {
            this.client = client;
        }

        public void Process()
        {
            //TODO: process request
        }

    }
}
