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
            string FilePath = HttpService.RootCatalog + request.Filename;
            bool _isDirectory = false;
            FileStream fileStream;
            long contentLength;

            FileAttributes attr = File.GetAttributes(FilePath);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                _isDirectory = true;
                string[] files = Directory.GetFiles(FilePath);
                string[] directories = Directory.GetDirectories(FilePath);
                string[] contents = new string[files.Length + directories.Length];
                directories.CopyTo(contents, 0);
                files.CopyTo(contents, directories.Length);
                return ReturnDirectory(contents, FilePath);
            }
            else
            {
                try
                {
                    fileStream = new FileStream(FilePath, FileMode.Open);
                    FileInfo info = new FileInfo(FilePath);
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
                return ReturnFile(fileStream, FilePath, contentLength);
            }

//            try
//            {
//                fileStream = new FileStream(FilePath, FileMode.Open);
//                FileInfo info = new FileInfo(FilePath);
//                contentLength = info.Length;
//            }
//            catch (ArgumentException)
//            {
//                return _response404;
//            }
//            catch (FileNotFoundException)
//            {
//                return _response404;
//            }
//            catch (DirectoryNotFoundException)
//            {
//                return _response404;
//            }
//
//            HttpResponse response = new HttpResponse(200, "OK");
//            response.AddProperty("Content-Type", GetContentType(FilePath));
//            response.AddProperty("Content-Length", contentLength);
//            response.AddProperty("Server", "BestServer.");
//
//            response.ContentFile = fileStream;
//            return response;
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
                          <title>Index of " + filePath + @"</title>
                          <style type=""text/css""></style>
                      </head>
                      <body>
                          <h1>Index of " + filePath + @"</h1>
                          <pre>
                              <hr>");
            foreach (string item in contents)
            {
                html.WriteLine("<a href=\"#" + item + "\"" + ">" + item + "</a>");
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
