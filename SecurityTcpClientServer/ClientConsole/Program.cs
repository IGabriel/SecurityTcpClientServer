using System;
using log4net;
using Common;
using SecurityClient;

namespace ClientConsole
{
    class Program
    {
        private static ILog _logger;

        static void Main(string[] args)
        {
            Common.LoggerFactory.Initialize();
            _logger = Common.LoggerFactory.GetLogger(typeof(Program));

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
            _logger.Info("Connect to a SSL server.");
            _logger.Info("ClientServer [server name] [port] [X509Certificate file path]");

            // Console.WriteLine("Connect to a SSL server.");
            // Console.WriteLine("ClientServer [server name] [port] [X509Certificate file path]");

            Console.WriteLine("Yes?");
        }
    }
}
