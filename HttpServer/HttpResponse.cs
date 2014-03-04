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
        public Stream ContentFile;
        public string Content;

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
            if (ContentFile != null)
            {
                writer.WriteLine();
                writer.Flush();
                ContentFile.CopyTo(stream);
                ContentFile.Close();
            }
            else if (Content != null)
            {
                writer.WriteLine();
                writer.WriteLine(Content);
                writer.Flush();
            }
            else
            {
                writer.Flush();
            }
        }
    }
}
