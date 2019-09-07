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
                && int.TryParse(args[1], out port)
                && !string.IsNullOrEmpty(args[2]))
            {
                string serverName = args[0];
                string certificateFile = args[2];

                _logger.InfoFormat("Connecting server: '{0}'; port: '{1}', certificate file: '{2}'.",
                    serverName, port, certificateFile);


                using (SslTcpClient client = new SslTcpClient(serverName, port, certificateFile))
                {
                    client.Connect();
                }
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
        }
    }
}
