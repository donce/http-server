using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    /// <summary>
    /// The class that reads the request headers
    /// </summary>
    class ReadingRequest
    {
        private readonly Stream stream;
        private readonly StreamReader reader;

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="stream">Input/output stream for reading and outputting the request headers</param>
        public ReadingRequest(Stream stream)
        {
            this.stream = stream;
            reader = new StreamReader(stream);
        }

        /// <summary>
        /// Reads the request headers
        /// </summary>
        /// <returns>Returns an HttpRequest object with headers</returns>
        public HttpRequest Read()
        {
            string[] lines = ReadLines();
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            HttpRequest request;
            try
            {
                request = new HttpRequest(lines);
            }
            catch (ArgumentException)
            {
                throw new BadRequestException();
            }

            if (request.Method == HttpRequest.Methods.POST)
            {
                request.Arguments = ReadPOSTContent(request);
                foreach (KeyValuePair<string, string> pair in request.Arguments)
                {
                    Console.WriteLine(pair.Key + " -> " + pair.Value);
                }
                //TODO: make use of POST arguments
            }

            return request;
        }

        /// <summary>
        /// Reads request headers line by line
        /// </summary>
        /// <returns>Returns an array of request header strings</returns>
        private string[] ReadLines()
        {
            List<string> lines = new List<string>();
            string line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                lines.Add(line);
                line = reader.ReadLine();
            }
            return lines.ToArray();
        }

        private IDictionary<string, string> ReadPOSTContent(HttpRequest request)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string item in ReadContent(request).Split('&'))
            {
                int pos = item.IndexOf('=');
                string key = item.Substring(0, pos);
                string value = item.Substring(pos + 1);
                dict[key] = value;
            }
            return dict;
        }

        private string ReadContent(HttpRequest request)
        {
            //TODO: Content-Length constant
            if (!request.Headers.ContainsKey("Content-Length"))
            {
                return null;
            }
            int length;
            try
            {
                length = Convert.ToInt32(request.Headers["Content-Length"]);
            }
            catch (Exception)
            {
                return null;
            }
            char[] buffer = new char[length];
            reader.Read(buffer, 0, length);
            return new string(buffer);
        }

    }

    class BadRequestException : Exception
    {
        
    }
}
