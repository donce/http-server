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

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="stream">Input/output stream for reading and outputting the request headers</param>
        public ReadingRequest(Stream stream)
        {
            this.stream = stream;
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
            return request;
        }

        /// <summary>
        /// Reads request headers line by line
        /// </summary>
        /// <returns>Returns an array of request header strings</returns>
        private string[] ReadLines()
        {
            StreamReader reader = new StreamReader(stream);
            List<string> lines = new List<string>();
            string line = reader.ReadLine();
            while (!String.IsNullOrEmpty(line))
            {
                lines.Add(line);
                line = reader.ReadLine();
            }
            return lines.ToArray();
        }

    }

    class BadRequestException : Exception
    {
        
    }
}
