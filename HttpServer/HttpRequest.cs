using System;
using log4net;
using log4net.Config;

namespace HttpServer
{
    class HttpRequest
    {

        private static  readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");
        public enum Methods
        {
            GET
        };

        public HttpRequest(string line)
        {
            log4net.Config.XmlConfigurator.Configure();
            if (String.IsNullOrEmpty(line))
                throw new ArgumentException();
            string[] requestWords = line.Split(' ');
            if (requestWords.Length != 3)
                throw new ArgumentException();

            Method = requestWords[0];
            Filename = requestWords[1];
            Protocol = requestWords[2];

            if (!Method.Equals("GET"))//TODO: use enum
            {
                errorLog.Error("Only GET requests are supported");
                throw new MethodException("Only GET requests are supported", "Method");
            }

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

        public readonly string Method;
        public readonly string Filename;
        public readonly string Protocol;

    }
}
