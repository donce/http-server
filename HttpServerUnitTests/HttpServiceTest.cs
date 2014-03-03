using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using HttpServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpServerUnitTests
{
    [TestClass]
    public class HttpServiceTest
    {
        private const string CrLf = "\r\n";

        [TestMethod]
        public void TestConstructor()
        {
            try
            {
                new HttpService(null);
                Assert.Fail();
            }
            catch (ArgumentNullException)
            {
                //empty
            }
        }

        [TestMethod]
        public void TestGet()
        {
            String line = GetFirstLine("GET /file.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 200 OK", line);

            line = GetFirstLine("GET /fileDoesNotExist.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 404 Not Found", line);
        }

        [TestMethod]
        public void TestGetIllegalRequest()
        {
            String line = GetFirstLine("GET /file.txt HTTP 1.0");
            Assert.AreEqual("HTTP/1.0 400 Illegal request", line);
        }

        [TestMethod]
        public void TestGetIllegalMethodName()
        {
            String line = GetFirstLine("PLET /file.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 400 Illegal request", line);
        }

//        [TestMethod]
        public void TestGetIllegalProtocol()
        {
            String line = GetFirstLine("GET /file.txt HTTP/1.2");
            Assert.AreEqual("HTTP/1.0 400 Illegal protocol", line);
        }

        [TestMethod]
        public void TestMethodNotImplemented()
        {
            String line = GetFirstLine("POST /file.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 200 xxx", line);
        }


        private static String GetFirstLine(String request)
        {
            TcpClient client = new TcpClient("localhost", Server.DefaultPort);
            NetworkStream networkStream = client.GetStream();

            StreamWriter toServer = new StreamWriter(networkStream, Encoding.UTF8);
            toServer.Write(request + CrLf);
            toServer.Write(CrLf);
            toServer.Flush();

            StreamReader fromServer = new StreamReader(networkStream);
            String firstline = fromServer.ReadLine();
            toServer.Close();
            fromServer.Close();
            client.Close();
            return firstline;

        }
    }
}
