using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using log4net;
using log4net.Config;

namespace HttpServer
{
    /// <summary>
    /// Http request class
    /// </summary>
    public class HttpRequest
    {
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");

        private const string INDEX_FILENAME = "/index.html";

        public enum Methods
        {
            GET,
            POST
        };

        public Methods Method { get; private set; }
        public string Filename { get; private set; }
        public string Protocol { get; private set; }

        public IDictionary<string, string> Headers = new Dictionary<string, string>();
        public IDictionary<string, string> Arguments = new Dictionary<string, string>();

        /// <summary>
        /// Constructor of the HttpRequest class
        /// </summary>
        /// <param name="line">The HTTP request string</param>
        public HttpRequest(string[] lines)
        {
            if (lines == null)
                throw new ArgumentNullException();
            if (lines.Length == 0)
            {
                throw new BadRequestException();
            }
            ParseRequestLine(lines[0]);
            for (int i = 1; i < lines.Length; ++i)
                ParseHeaderLine(lines[i]);
        }

        /// <summary>
        /// A constructor for the HttpRequest object
        /// </summary>
        /// <param name="requestLine">HTTP request string</param>
        public HttpRequest(string requestLine) : this(new[] {requestLine})
        {
        }

        /// <summary>
        /// Parses the HTTP request
        /// </summary>
        /// <param name="line">HTTP request string</param>
        private void ParseRequestLine(string line)
        {
            XmlConfigurator.Configure();
            if (String.IsNullOrWhiteSpace(line))
                throw new ArgumentException();
            string[] requestWords = line.Split(' ');
            if (requestWords.Length != 3)
                throw new ArgumentException();

            Methods method;
            if (!Methods.TryParse(requestWords[0], out method))
            {
                errorLog.Error("Only GET requests are supported");
                throw new MethodException("Only GET requests are supported", "Method");
            }
            Method = method;

            Filename = requestWords[1];
            Filename = Uri.UnescapeDataString(Filename);
            Filename = Filename.Replace('+', ' ');
            if (Filename.Equals("/"))
                Filename = INDEX_FILENAME;

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

        /// <summary>
        /// Parses a header line
        /// </summary>
        /// <param name="line">Header line string</param>
        private void ParseHeaderLine(string line)
        {
            int pos = line.IndexOf(':');
            string key = line.Substring(0, pos);
            string value = line.Substring(pos + 1).TrimStart(new char[] {' '});
            Headers[key] = value;
        }
    }
}