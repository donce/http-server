using System;

namespace HttpServer
{
    class HttpRequest
    {
        public enum Methods
        {
            GET
        };

        public HttpRequest(string line)
        {
            //TODO: new exception classes
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
                throw new MethodException("Only GET requests are supported", "Method");
            }

            string[] protocolWords = Protocol.Split('/');
            if (protocolWords.Length != 2)
                throw new ProtocolException("Invalid protocol format", "protocolWords");
            if (!protocolWords[0].Equals("HTTP"))
                throw new ProtocolException("Only HTTP is supported", "protocolWords[0]");

            //TODO: check "HTTP/text"
            try
            {
                decimal protocolVersion = decimal.Parse(protocolWords[1]);
                Console.WriteLine("Version:");
                Console.WriteLine(protocolVersion);
                if (protocolVersion < 1)
                    throw new ProtocolException("Invalid HTTP version", "protocolVersion");
            }
            catch (FormatException)
            {
                throw new ProtocolException("Invalid version format", "protocolVersion");
            }
        }

        public readonly string Method;
        public readonly string Filename;
        public readonly string Protocol;

    }
}
