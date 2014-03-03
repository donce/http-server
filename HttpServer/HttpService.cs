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

        private static readonly IDictionary<string, string> contentTypes;
        private const string defaultContentType = "application/octet-stream";

        static HttpService()
        {
            contentTypes = new Dictionary<string, string>();
            contentTypes["html"] = "text/html";
            contentTypes["htm"] = "text/html";
            contentTypes["doc"] = "application/msword";
            contentTypes["png"] = "image/png";
            contentTypes["gif"] = "image/gif";
            contentTypes["jpg"] = "image/jpeg";
            contentTypes["pdf"] = "application/pdf";
            contentTypes["css"] = "text/css";
            contentTypes["xml"] = "text/xml";
            contentTypes["jar"] = "application/x-java-archive";
        }

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

            string filename;
            try
            {
                filename = GetFile(lines[0]);
            }
            catch (ArgumentException)
            {
                writer.WriteLine("HTTP/1.0 400 Illegal request");
                client.Close();
                return;
            }

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
            writer.WriteLine("Content-Type: " + GetContentType(filename));
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
            //TODO: new exception classes
            if (String.IsNullOrEmpty(request))
                throw new ArgumentException();
            string[] requestWords = request.Split(' ');
            if (requestWords.Length != 3)
                throw new ArgumentException();

            string method = requestWords[0];
            string filename = requestWords[1];
            string protocol = requestWords[2];

            if (!method.Equals("GET"))
            {
                throw new ArgumentException("Only GET requests are supported", "request");
            }

            string[] protocolWords = protocol.Split('/');
            if (protocolWords.Length != 2)
                throw new ArgumentException();
            if (!protocolWords[0].Equals("HTTP"))
                throw new ArgumentException();

            //TODO: check "HTTP/text"
            decimal protocolVersion = decimal.Parse(protocolWords[1]);
            Console.WriteLine("Version:");
            Console.WriteLine(protocolVersion);
            if (protocolVersion < 1)
                throw new Exception();
            return RootCatalog + filename;
        }

        private static string GetContentType(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException("Null or empty", "filename");
            string extension = Path.GetExtension(filename).Substring(1);
            if (contentTypes.ContainsKey(extension))
                return contentTypes[extension];
            return defaultContentType;
        }
    }
}
