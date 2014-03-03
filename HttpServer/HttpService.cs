using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HttpService
    {
        private TcpClient client;

        public HttpService(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            this.client = client;
        }

        public void Process()
        {
            Console.WriteLine("Client connected");
            Stream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            string line = reader.ReadLine();
            //                while (line != null)
            //                {
            Console.WriteLine(line);
            //                    line = reader.ReadLine();
            //                }

            string filename = GetFile(line);

            string content = "You've requested " + filename;
            writer.WriteLine("HTTP/1.0 200 OK");
            writer.WriteLine("");
            writer.WriteLine(content);

            writer.Flush();

            client.Close();//TODO: in finally
        }

        public string GetFile(string getRequest)
        {
            string[] requestWords = getRequest.Split(' ');
            if (!requestWords[0].Equals("GET"))
            {
                throw (new ArgumentException("Only GET requests are supported"));
            }
            string pathToFile = requestWords[1];
            return pathToFile;
            //            return "index.html";
        }

    }
}
