using System;
using System.Collections.Generic;
using System.IO;

namespace HttpServer
{
    public class HandlingRequest
    {
        private static readonly IDictionary<string, string> contentTypes;
        private const string defaultContentType = "application/octet-stream";

        private static readonly HttpResponse _response404;

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

            _response404 = new HttpResponse(404, "Not Found");
            _response404.Content = "Page not found.";
        }


        public static HttpResponse ProcessRequest(HttpRequest request)
        {
            string filePath = HttpService.RootCatalog + request.Filename;
            FileAttributes attr;

            try
            {
                attr = File.GetAttributes(filePath);
            }
            catch (DirectoryNotFoundException)
            {
                return _response404;
            }
            catch (FileNotFoundException)
            {
                return _response404;
            }
            catch (ArgumentException)
            {
                return _response404;
            }

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                string[] files = Directory.GetFiles(filePath);
                string[] directories = Directory.GetDirectories(filePath);
                string[] contents = new string[files.Length + directories.Length];
                directories.CopyTo(contents, 0);
                files.CopyTo(contents, directories.Length);
                return ReturnDirectory(contents, filePath);
            }

            FileStream fileStream;
            long contentLength;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                FileInfo info = new FileInfo(filePath);
                contentLength = info.Length;
            }
            catch (ArgumentException)
            {
                return _response404;
            }
            catch (FileNotFoundException)
            {
                return _response404;
            }
            catch (DirectoryNotFoundException)
            {
                return _response404;
            }
            return ReturnFile(fileStream, filePath, contentLength);
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

        public static HttpResponse ReturnDirectory(string[] contents, string filePath)
        {
            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-type", "text/html");
            response.AddProperty("Server", "BestServer");
            response.Content = MakeHtml(contents, filePath);
            return response;
        }

        public static HttpResponse ReturnFile(FileStream fileStream, string filePath, long contentLength)
        {
            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-Type", GetContentType(filePath));
            response.AddProperty("Content-Length", contentLength);
            response.AddProperty("Server", "BestServer.");

            response.ContentFile = fileStream;
            return response;
        }

        public static string MakeHtml(string[] contents, string filePath)
        {
            StringWriter html = new StringWriter();
            html.WriteLine(
                @"<html>
                      <head>
                          <title>Index of /" + filePath.Split('/')[filePath.Split('/').Length - 2] + @"</title>
                          <style type=""text/css""></style>
                      </head>
                      <body>
                          <h1>Index of /" + filePath.Split('/')[filePath.Split('/').Length - 2] + @"</h1>
                          <pre>
                              <hr>");
            foreach (string item in contents)
            {
                html.WriteLine("<a href=\"#\"" + ">" + item.Split('/')[item.Split('/').Length - 1] + "</a>");
            }
            html.WriteLine(
                @"        </pre>
                          <hr>
                          <address>BestServer</address>
                      </body>
                  </html>");
            return html.ToString();
        }

    }
}
