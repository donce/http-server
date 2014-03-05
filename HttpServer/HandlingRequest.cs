using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace HttpServer
{
    /// <summary>
    /// The class handling/parsing the HTTP request
    /// </summary>
    public class HandlingRequest
    {
        private static readonly IDictionary<string, string> contentTypes;
        private static readonly ILog errorLog = LogManager.GetLogger("ErrorLogger");
        private static readonly HttpResponse _response404;

        /// <summary>
        /// The constructor of the class
        /// </summary>
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

        /// <summary>
        /// Processes the request to be an appropriate response
        /// </summary>
        /// <param name="request">HTTP request</param>
        /// <returns></returns>
        public static HttpResponse ProcessRequest(HttpRequest request)
        {
            log4net.Config.XmlConfigurator.Configure();
            string filePath = ServerClass.Configuration.RootPath + request.Filename;
            string indexPath = ServerClass.Configuration.RootPath + "/index.html";
            FileAttributes attr;
            FileStream fileStream;
            long contentLength;

            if (request.Filename.Equals("/"))
            {
                try
                {
                    fileStream = new FileStream(indexPath, FileMode.Open);
                    FileInfo info = new FileInfo(indexPath);
                    contentLength = info.Length;
                }
                catch (DirectoryNotFoundException)
                {
                    errorLog.Error("Directory does not exist");
                    return _response404;
                }
                catch (FileNotFoundException)
                {
                    errorLog.Error("File does not exist");
                    return _response404;
                }
                catch (ArgumentException)
                {
                    errorLog.Error("Illegal characters in folder or file name");
                    return _response404;
                }

                return ReturnFile(fileStream, indexPath, contentLength);
            }

            try
            {
                attr = File.GetAttributes(filePath);
            }
            catch (DirectoryNotFoundException)
            {
                errorLog.Error("Directory does not exist");
                return _response404;
            }
            catch (FileNotFoundException)
            {
                errorLog.Error("File does not exist");
                return _response404;
            }
            catch (ArgumentException)
            {
                errorLog.Error("Illegal characters in folder or file name");
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

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open);
                FileInfo info = new FileInfo(filePath);
                contentLength = info.Length;
            }
            catch (ArgumentException)
            {
                errorLog.Error("Illegal characters in folder or file name");
                return _response404;
            }
            catch (FileNotFoundException)
            {
                errorLog.Error("File does not exist");
                return _response404;
            }
            catch (DirectoryNotFoundException)
            {
                errorLog.Error("Directory does not exist");
                return _response404;
            }
            return ReturnFile(fileStream, filePath, contentLength);
        }

        /// <summary>
        /// Gets the conten type of the file
        /// </summary>
        /// <param name="filename">The name of the file</param>
        /// <returns></returns>
        private static string GetContentType(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException("Null or empty", "filename");
            string extension = Path.GetExtension(filename).Substring(1);
            if (contentTypes.ContainsKey(extension))
                return contentTypes[extension];
            return ServerClass.Configuration.DefaultContentType;
        }

        /// <summary>
        /// Returns the content to display if the URL is a directory
        /// </summary>
        /// <param name="contents">Array of directory contents</param>
        /// <param name="filePath">The file path of the directory</param>
        /// <returns></returns>
        public static HttpResponse ReturnDirectory(string[] contents, string filePath)
        {
            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-type", "text/html");
            response.AddProperty("Server", "BestServer");
            response.Content = MakeHtml(contents, filePath);
            return response;
        }

        /// <summary>
        /// Returns the files content if it's of valid content type
        /// </summary>
        /// <param name="fileStream">The file stream for writing and outputting the file</param>
        /// <param name="filePath">The path of the file</param>
        /// <param name="contentLength">The content legth of the file</param>
        /// <returns></returns>
        public static HttpResponse ReturnFile(FileStream fileStream, string filePath, long contentLength)
        {
            HttpResponse response = new HttpResponse(200, "OK");
            response.AddProperty("Content-Type", GetContentType(filePath));
            response.AddProperty("Content-Length", contentLength);
            response.AddProperty("Server", "BestServer.");

            response.ContentFile = fileStream;
            return response;
        }

        /// <summary>
        /// Generate the HTML file of directory contents
        /// </summary>
        /// <param name="contents">Array of directories contents</param>
        /// <param name="filePath">The file path of the directory</param>
        /// <returns></returns>
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