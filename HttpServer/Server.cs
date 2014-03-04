
namespace HttpServer
{
    
    public class Server
    {
        public static bool accepting = true;
        public static string configurationFilename = "../../../Configuration.xml";

        static void Main(string[] args)
        {
            ServerClass server = new ServerClass(configurationFilename);
            server.Start();
        }
    }
}
