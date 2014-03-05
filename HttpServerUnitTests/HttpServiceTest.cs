using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpServerUnitTests
{
    [TestClass]
    public class HttpServiceTest
    {
        private const string CrLf = "\r\n";
        private static ServerClass _server;

        private static Configuration _configuration;

        [ClassInitialize]
        public static void StartServer(TestContext context)
        {
            _configuration = new Configuration(Server.configurationFilename);
            _server = new ServerClass(Server.configurationFilename);
            Task.Factory.StartNew(() => _server.Start());
        }

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
        public void TestPost()
        {
            String line = GetFirstLine("POST /file.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 200 OK", line);

            line = GetFirstLine("POST /fileDoesNotExist.txt HTTP/1.0");
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

        [TestMethod]
        public void TestGetIllegalProtocol()
        {
            String line = GetFirstLine("GET /file.txt HTTP/0.9");
            Assert.AreEqual("HTTP/1.0 400 Illegal protocol", line);
        }

        [TestMethod]
        public void TestMethodNotImplemented()
        {
            String line = GetFirstLine("PUT /file.txt HTTP/1.0");
            Assert.AreEqual("HTTP/1.0 400 Illegal request", line);
        }

        [TestMethod]
        public void TestMultipleFileRequests()
        {
            const string QUERY = "GET /image.png HTTP/1.0";
            const string EXPECTED_RESULT = "HTTP/1.0 200 OK";

            int failed = 0;

            List<Task> actions = new List<Task>();
            for (int i = 0; i < 10; ++i)
            {
                Action action = () =>
                {
                    if (EXPECTED_RESULT != GetFirstLine(QUERY))
                        failed++;
                };
                actions.Add(Task.Run(action));
            }
            Task.WaitAll(actions.ToArray());
            Assert.AreEqual(0, failed);
        }

        [ClassCleanup]
        public static void StopServer()
        {
            _server.Stop();
        }

        private static String GetFirstLine(String request)
        {
            TcpClient client = new TcpClient("localhost", _configuration.Port);
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
