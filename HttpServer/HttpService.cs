using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using log4net;

namespace HttpServer
{
    /// <summary>
    /// Class handling the client connection to the server
    /// </summary>
    public class HttpService
    {
        private TcpClient client;

        private Stream stream;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");

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
            log4net.Config.XmlConfigurator.Configure();
            log.Info("Client connected");

            ReadingRequest reading = new ReadingRequest(stream);
            HttpRequest request;
            try
            {
                request = reading.Read();
            }
            catch (BadRequestException)
            {
                errorLog.Error("Illegal request");
                return new HttpResponse(400, "Illegal request");
            }
            catch (MethodException)
            {
                errorLog.Error("Illegal method");
                return new HttpResponse(400, "Illegal request");
            }
            catch (ProtocolException)
            {
                errorLog.Error("Illegal protocol");
                return new HttpResponse(400, "Illegal protocol");
            }

            //TODO: log
            if (request.GetArguments.Count > 0)
            {
                Console.WriteLine("GET arguments:");
                foreach (KeyValuePair<string, string> pair in request.GetArguments)
                {
                    Console.WriteLine(pair.Key + ": " + pair.Value);
                }
            }
            if (request.PostArguments.Count > 0)
            {
                Console.WriteLine("POST arguments:");
                foreach (KeyValuePair<string, string> pair in request.PostArguments)
                {
                    Console.WriteLine(pair.Key + ": " + pair.Value);
                }
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