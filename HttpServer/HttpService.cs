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

        private StreamReader reader;
        private StreamWriter writer;

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
            Stream stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public void Process()
        {
            Console.WriteLine("Client connected");
            Stream stream = client.GetStream();

            String[] lines = ReadRequest();
            if (lines.Length == 0)
                throw new Exception();

            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            HttpRequest request;
            try
            {
                request = new HttpRequest(lines[0]);
            }
            catch (ArgumentException)
            {
                writer.WriteLine("HTTP/1.0 400 Illegal request");
                client.Close();
                return;
            }

            string filename = RootCatalog + request.Filename;

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
            AddProperty("Content-Type", GetContentType(filename));
            writer.WriteLine("");
            fileStream.CopyTo(stream);
            fileStream.Close();

            client.Close();//TODO: in finally
        }

        private string[] ReadRequest()
        {
            List<string> lines = new List<string>();
            string line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                lines.Add(line);
                line = reader.ReadLine();
            }
            return lines.ToArray();
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

        private void AddProperty(string key, string value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Null or empty", "key");
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("Null or empty", "value");
            writer.Write(key + ": " + value);
        }
    }
}
