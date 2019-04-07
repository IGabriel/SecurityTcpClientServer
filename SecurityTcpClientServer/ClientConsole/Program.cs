using System;
using SecurityClient;

namespace ClientConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int port;
            if (args != null && args.Length == 3
                && !string.IsNullOrEmpty(args[0])
                && !int.TryParse(args[1], out port)
                && !string.IsNullOrEmpty(args[2]))
            {
                string serverName = args[0];

                SslTcpClient client = new SslTcpClient(args[0], port, args[2]);
                //client.Connect()

            }
            else
            {
                ShowUsage();
            }

        }

        private static void ShowUsage()
        {
            Console.WriteLine("Connect to a SSL server.");
            Console.WriteLine("ClientServer [server name] [port] [X509Certificate file path]");
        }
    }
}
