using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class HandlingRequest
    {
        private static readonly IDictionary<string, string> contentTypes;
        private const string defaultContentType = "application/octet-stream";

        static HandlingRequest()
        {
            contentTypes = new Dictionary<string, string>();
            contentTypes["html"] = "text/html";
            contentTypes["htm"] = "text/html";
            contentTypes["txt"] = "text/plain";
            contentTypes["doc"] = "application/msword";
            contentTypes["png"] = "image/png";
            contentTypes["gif"] = "image/gif";
            contentTypes["jpg"] = "image/jpeg";
            contentTypes["pdf"] = "application/pdf";
            contentTypes["css"] = "text/css";
            contentTypes["xml"] = "text/xml";
            contentTypes["jar"] = "application/x-java-archive";
        }

        public static HttpResponse ProcessRequest(HttpRequest request)
        {
            string filename = HttpService.RootCatalog + request.Filename;

            FileStream fileStream;

            long contentLength;

            try
            {
                fileStream = new FileStream(filename, FileMode.Open);
                FileInfo info = new FileInfo(filename);
                contentLength = info.Length;
            }
            catch (ArgumentException)
            {
                return new HttpResponse(404, "Not Found");
            }
            catch (FileNotFoundException)
            {
                return new HttpResponse(404, "Not Found");
            }
            catch (DirectoryNotFoundException)
            {
                return new HttpResponse(404, "Not Found");
            }

            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-Type", GetContentType(filename));
            response.AddProperty("Content-Length", contentLength);
            response.AddProperty("Server", "BestServer.");

            response.Content = fileStream;
            return response;
        }

        private static string GetContentType(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException("Null or empty", "filename");
            string extension = Path.GetExtension(filename).Substring(1);
            if (contentTypes.ContainsKey(extension))
                return contentTypes[extension];
            return defaultContentType;
        }


    }
}
