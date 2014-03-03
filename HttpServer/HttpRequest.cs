using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                throw new ArgumentException("Only GET requests are supported", "line");
            }

            string[] protocolWords = Protocol.Split('/');
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
        }

        public readonly string Method;
        public readonly string Filename;
        public readonly string Protocol;

    }
}
