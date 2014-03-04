using System;
using System.IO;
using log4net;
using log4net.Config;

namespace HttpServer
{
    public class HttpRequest
    {

        private static  readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");
        public enum Methods
        {
            GET
        };

        public readonly Methods Method;
        public readonly string Filename;
        public readonly string Protocol;

        public HttpRequest(string line)
        {
            log4net.Config.XmlConfigurator.Configure();
            if (line == null)
                throw new ArgumentNullException();
            if (String.IsNullOrWhiteSpace(line))
                throw new ArgumentException();
            string[] requestWords = line.Split(' ');
            if (requestWords.Length != 3)
                throw new ArgumentException();

            if (!Methods.TryParse(requestWords[0], out Method))
            {
                errorLog.Error("Only GET requests are supported");
                throw new MethodException("Only GET requests are supported", "Method");
            }
            Filename = requestWords[1];
            Filename = Uri.UnescapeDataString(Filename);
            Filename = Filename.Replace('+', ' ');
            Protocol = requestWords[2];

            string[] protocolWords = Protocol.Split('/');
            if (protocolWords.Length != 2)
            {
                errorLog.Error("Invalid protocol format");
                throw new ProtocolException("Invalid protocol format", "protocolWords");
            }
            if (!protocolWords[0].Equals("HTTP"))
            {
                errorLog.Error("Only HTTP is supported");
                throw new ProtocolException("Only HTTP is supported", "protocolWords[0]");
            }

            try
            {
                decimal protocolVersion = decimal.Parse(protocolWords[1]);
                Console.WriteLine("Version:");
                Console.WriteLine(protocolVersion);
                if (protocolVersion < 1)
                {
                    errorLog.Error("Invalid HTTP version");
                    throw new ProtocolException("Invalid HTTP version", "protocolVersion");
                }
            }
            catch (FormatException)
            {
                errorLog.Error("Invalid HTTP version format");
                throw new ProtocolException("Invalid HTTP version format", "protocolVersion");
            }
        }
    }
}
