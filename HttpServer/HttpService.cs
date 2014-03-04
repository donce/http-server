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
            contentTypes["txt"] = "text/plain";
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
//            writer = new StreamWriter(stream);
        }

        private HttpResponse GetResponse()
        {
            Console.WriteLine("Client connected");

            ReadingRequest reading = new ReadingRequest(stream);
            HttpRequest request;
            try
            {
                request = reading.Read();
//                    TODO:URL decoding
//                    Console.WriteLine(DecodeUrlString(reading.Read());
//                    request = new HttpRequest(DecodeUrlString(lines[0]));
            }
            catch (BadRequestException)
            {
                return new HttpResponse(400, "Illegal request");
            }


            string filename = RootCatalog + request.Filename;

            FileStream fileStream;

            long contentLength;

            try
            {
                //TODO: using
                fileStream = new FileStream(filename, FileMode.Open);
                FileInfo info = new FileInfo(filename);
                contentLength = info.Length;
            }
            catch (ArgumentException)
            {
                return new HttpResponse(404, "Not Found");
            }
            catch (FileNotFoundException)
            {
                return new HttpResponse(404, "Not Found");
            }
            catch (DirectoryNotFoundException)
            {
                return new HttpResponse(404, "Not Found");
            }

//                WriteResponse(200, "OK");
            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-Type", GetContentType(filename));
            response.AddProperty("Content-Length", contentLength);
            response.AddProperty("Server", "BestServer.");

            //TODO: close FileStream

            response.Content = fileStream;
            return response;
//                response.Write(stream);
//                writer.WriteLine("");
//                writer.Flush();
//                fileStream.CopyTo(stream);
//                fileStream.Close();
        }

        public void Process()
        {
            try
            {
                HttpResponse response = GetResponse();
                response.Write(stream);
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

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
    }
}
