using ClientServerLibrary;

namespace ClientConsole
{
    class Program
    {
        private static ILogger _logger;

        static void Main(string[] args)
        {
            LoggerFactory.Initialize();
            _logger = LoggerFactory.GetLogger(typeof(Program));

            if (args.Length != 2)
            {
                ShowUsage();
                return;
            }

            int port;
            if (!int.TryParse(args[1], out port))
            {
                ShowUsage();
                return;
            }

            string machineName = args[0];
            _logger.InfoFormat("Connecting server: '{0}'; port: '{1}'.", machineName, port);

            using (SecurityClient client = new SecurityClient(machineName, port))
            {
                client.Connect();

                //System.Console.ReadLine();
            }            
        }

        private static void ShowUsage()
        {
            _logger.Info("Connect to server.");
            _logger.Info("ClientConsole [server name] [port]");
        }

    }
}
