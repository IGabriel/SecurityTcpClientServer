using System;
using System.Threading;
using ClientServerLibrary;

namespace ServerConsole
{
    class Program
    {
        private static ILogger _logger;

        static void Main(string[] args)
        {
            LoggerFactory.Initialize();
            _logger = LoggerFactory.GetLogger(typeof(Program));

            if (args.Length != 1)
            {
                ShowUsage();
                return;
            }

            int port;
            if (int.TryParse(args[0], out port))
            {
                _logger.InfoFormat("Listening port '{0}'.",
                    port);

                var source = new CancellationTokenSource();
                SecurityListener service = new SecurityListener(Convert.ToInt32(port), source.Token);
                service.Start(); 

                Quit();
            }
            else
            {
                ShowUsage();
            }
        }

        private static void ShowUsage()
        {
            _logger.Info("Listen the TCP port:");
            _logger.Info("SecurityListener [port]");
        }

        private static void Quit()
        {
            const string keyword = "quit";
            _logger.InfoFormat("Type '{0}' to stop.", keyword);

            string input = Console.ReadLine();
            if (!string.Equals(keyword, input, StringComparison.InvariantCultureIgnoreCase))
            {
                Quit();
            }
        }
    }
}
