using System;
using HttpServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpServerUnitTests
{
    [TestClass]
    public class HttpServiceTest
    {
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
    }
}
