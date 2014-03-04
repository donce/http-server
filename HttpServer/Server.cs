
namespace HttpServer
{
    
    public class Server
    {
        public const int DefaultPort = 8080;
        public static bool accepting = true;

        static void Main(string[] args)
        {
            ServerClass server = new ServerClass(DefaultPort);//TODO:port in config file
            server.Start();
        }
    }
}
