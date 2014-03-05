using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpServerUnitTests
{
    [TestClass]
    public class HttpRequestTest
    {

        [TestMethod]
        public void TestConstructor()
        {
            HttpRequest request = new HttpRequest("GET /file.txt HTTP/1.0");
            Assert.AreEqual(HttpRequest.Methods.GET, request.Method);
            Assert.AreEqual("/file.txt", request.Filename);
            Assert.AreEqual("HTTP/1.0", request.Protocol);
            
            try
            {
                new HttpRequest("");
                Assert.Fail();
            }
            catch (ArgumentException) { }

            try
            {
                new HttpRequest("  ");
                Assert.Fail();
            }
            catch (ArgumentException) { }

            try
            {
                new HttpRequest("XXX");
                Assert.Fail();
            }
            catch (ArgumentException) { }

            try
            {
                new HttpRequest("XXX / HTTP/1.0");
                Assert.Fail();
            }
            catch (MethodException) { }

            try
            {
                new HttpRequest("GET / XXXX/1.0");
                Assert.Fail();
            }
            catch (ProtocolException) { }

            try
            {
                new HttpRequest("GET / HTTP/0.9");
                Assert.Fail();
            }
            catch (ProtocolException) { }

//            HttpRequest request = new("GET /Anders+B%C3%B8rjessonl.txt HTTP/1.1")
            Assert.AreEqual("/Anders Børjessonl.txt", new HttpRequest("GET /Anders+B%C3%B8rjessonl.txt HTTP/1.1").Filename);
        }

    }
}
