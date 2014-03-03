using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            String[] lines = ReadRequest(stream);
            if (lines.Length == 0)
                throw new Exception();

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            string filename = GetFile(lines[0]);
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open);
            }
            catch (FileNotFoundException)
            {
                writer.WriteLine("HTTP/1.0 404 Not Found");
                writer.WriteLine("");
                writer.WriteLine("Page not found");
                client.Close();//TODO: in finally
                return;
            }

            writer.WriteLine("HTTP/1.0 200 OK");
            writer.WriteLine("");
            fileStream.CopyTo(stream);
            fileStream.Close();

            client.Close();//TODO: in finally
        }

        private string[] ReadRequest(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            List<string> lines = new List<string>();
            string line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                lines.Add(line);
                line = reader.ReadLine();
            }
            return lines.ToArray();
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
