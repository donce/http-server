using System;
using System.IO;
using System.Net.Sockets;

namespace HttpServer
{
    public class HttpService
    {
        private static readonly string RootCatalog = "c:/www";

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

            string line = reader.ReadLine();
//            while (!reader.EndOfStream)
//            {
                Console.WriteLine("a:" + line);
//                line = reader.ReadLine();
//            }
            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            string filename = GetFile(line);
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                writer.WriteLine("HTTP/1.0 404 Not Found");
                client.Close();//TODO: in finally
                return;
            }

            writer.WriteLine("HTTP/1.0 200 OK");
            writer.WriteLine("");
            fileStream.CopyTo(stream);
            fileStream.Close();

            client.Close();//TODO: in finally
        }

        private string GetFile(string request)
        {
            if (String.IsNullOrEmpty(request))
                throw new ArgumentException();
            string[] requestWords = request.Split(' ');
            if (!requestWords[0].Equals("GET"))
            {
                throw (new ArgumentException("Only GET requests are supported", "request"));
            }
            string pathToFile = requestWords[1];
            return RootCatalog + pathToFile;
        }

    }
}
