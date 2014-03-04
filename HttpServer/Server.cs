
namespace HttpServer
{
    
    public class Server
    {
        public const int DefaultPort = 8080;
        public static bool accepting = true;

        static void Main(string[] args)
        {
            ServerClass server = new ServerClass();
            server.Start(DefaultPort);//TODO:port in config file
        }
    }
}
