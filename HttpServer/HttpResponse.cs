using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HttpResponse
    {
        private IDictionary<string, string> properties = new Dictionary<string, string>();

        public readonly int Status;
        public readonly string Message;
        public Stream Content;

        public HttpResponse(int status, string message)
        {
            Status = status;
            Message = message;
        }

        public void AddProperty(string key, Object value)
        {
            if (String.IsNullOrEmpty(key))
                throw new ArgumentException("Null or empty", "key");
            if (String.IsNullOrEmpty(value.ToString()))
                throw new ArgumentException("Null or empty", "value");
            properties[key] = value.ToString();
        }

        public void Write(Stream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("HTTP/1.0 " + Status.ToString() + " " + Message);
            foreach (KeyValuePair<string, string> pair in properties)
            {
                writer.WriteLine(pair.Key + ": " + pair.Value);
            }
            if (Content != null)
            {
                writer.WriteLine();
                writer.Flush();
                Content.CopyTo(stream);
                Content.Close();
            }
            else
            {
                if (Status == 404)
                {
                    writer.WriteLine();
                    writer.WriteLine("Page not found.");
                }
                writer.Flush();
            }
        }
    }
}
