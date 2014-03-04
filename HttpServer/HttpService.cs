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

        private Stream stream;
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
            stream = client.GetStream();
            writer = new StreamWriter(stream);
        }

        public void Process()
        {
            try
            {
                Console.WriteLine("Client connected");

                ReadingRequest reading = new ReadingRequest(stream);
                HttpRequest request;
                try
                {
                    request = reading.Read();
                }
                catch (BadRequestException)
                {
                    Write400();
                    return;
                }


                string filename = RootCatalog + request.Filename;

                FileStream fileStream;

                long contentLength;

                try
                {
                    fileStream = new FileStream(filename, FileMode.Open);
                    FileInfo info = new FileInfo(filename);
                    contentLength = info.Length;
                }
                catch (ArgumentException)
                {
                    Write404();
                    return;
                }
                catch (FileNotFoundException)
                {
                    Write404();
                    return;
                }
                catch (DirectoryNotFoundException)
                {
                    Write404();
                    return;
                }

                WriteResponse(200, "OK");
                AddProperty("Content-Type", GetContentType(filename));
                AddProperty("Content-Length", contentLength);
                AddProperty("Server", "Best HTTP server ever.");
                writer.WriteLine("");
                writer.Flush();
                fileStream.CopyTo(stream);
                fileStream.Close();
            }
            finally
            {
                client.Close();
            }
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

        private void AddProperty(string key, Object value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Null or empty", "key");
            if (String.IsNullOrEmpty(value.ToString()))
                throw new ArgumentException("Null or empty", "value");
            writer.WriteLine(key + ": " + value);
        }

        private void WriteResponse(int status, string message)
        {
            writer.WriteLine("HTTP/1.0 " + status.ToString() + " " + message);
        }

        private void Write400()
        {
            WriteResponse(400, "Illegal request");
        }

        private void Write404()
        {
            WriteResponse(404, "Not Found");
            writer.WriteLine("");
            writer.WriteLine("Page not found");
            writer.Flush();
        }
    }
}
