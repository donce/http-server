using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace HttpServer
{
    public class HttpService
    {
        public static readonly string RootCatalog = "c:/www";

        private TcpClient client;

        private Stream stream;
        private StreamWriter writer;


        public HttpService(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            this.client = client;
            stream = client.GetStream();
        }

        private HttpResponse GetResponse()
        {
            Console.WriteLine("Client connected");

            ReadingRequest reading = new ReadingRequest(stream);
            HttpRequest request;
            try
            {
                request = reading.Read();
//                TODO:URL decoding
//                Console.WriteLine(DecodeUrlString(reading.Read());
//                request = new HttpRequest(DecodeUrlString(lines[0]));
            }
            catch (BadRequestException)
            {
                return new HttpResponse(400, "Illegal request");
            }
            catch (MethodException)
            {
                return new HttpResponse(400, "Illegal request");
            }
            catch (ProtocolException)
            {
                return new HttpResponse(400, "Illegal protocol");
            }

            return HandlingRequest.ProcessRequest(request);

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

        private static string DecodeUrlString(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }
    }
}
