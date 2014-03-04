using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class ReadingRequest
    {
        private readonly Stream stream;

        public ReadingRequest(Stream stream)
        {
            this.stream = stream;
        }

        public HttpRequest Read()
        {
            string[] lines = ReadLines();
            if (lines.Length == 0)
            {
                throw new BadRequestException();
            }
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            HttpRequest request;
            try
            {
                request = new HttpRequest(lines[0]);
            }
            catch (ArgumentException)
            {
                throw new BadRequestException();
            }
            return request;
        }

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
