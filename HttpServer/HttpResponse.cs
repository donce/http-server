using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace HttpServer
{
    /// <summary>
    /// The server response class
    /// </summary>
    public class HttpResponse
    {
        private readonly IDictionary<string, string> properties = new Dictionary<string, string>();
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");
        public readonly int Status;
        public readonly string Message;
        public Stream ContentFile;
        public string Content;

        /// <summary>
        /// The constructor of the class
        /// </summary>
        /// <param name="status">The status code of the response</param>
        /// <param name="message">The status message</param>
        public HttpResponse(int status, string message)
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// Adds a property(i.e. header) to the HTTP response 
        /// </summary>
        /// <param name="key">The key of the properties dictionary</param>
        /// <param name="value">The value of the property</param>
        public void AddProperty(string key, Object value)
        {
            log4net.Config.XmlConfigurator.Configure();
            if (String.IsNullOrEmpty(key))
            {
                errorLog.Error("Null key when trying to add a property");
                throw new ArgumentException("Null or empty", "key");
            }
            if (String.IsNullOrEmpty(value.ToString()))
            {
                errorLog.Error("Null value when trying to add a property");
                throw new ArgumentException("Null or empty", "value");
            }
            properties[key] = value.ToString();
        }

        /// <summary>
        /// Write text or the content of a file to browser output
        /// </summary>
        /// <param name="stream">The output stream</param>
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