using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;

namespace HttpServer
{
    /// <summary>
    /// Class handling the client connection to the server
    /// </summary>
    public class HttpService
    {
        private TcpClient client;

        private Stream stream;
        private StreamWriter writer;

        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="client">The client connection</param>
        public HttpService(TcpClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            this.client = client;
            stream = client.GetStream();
        }

        /// <summary>
        /// Get the server response
        /// </summary>
        /// <returns>Returns the handled request from the server to the client</returns>
        private HttpResponse GetResponse()
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

        /// <summary>
        /// Sends the response back to the client
        /// </summary>
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
    }
}
