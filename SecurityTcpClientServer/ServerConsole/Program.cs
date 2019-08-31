using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Common;
using SecurityServer;


namespace ServerConsole
{
    class Program
    {
        private static ILog _logger;

        static void Main(string[] args)
        {
            Common.LoggerFactory.Initialize();
            _logger = Common.LoggerFactory.GetLogger(typeof(Program));

            SecurityAsyncService service;
            if (args.Length != 2)
            {
                ShowUsage();
                return;
            }

            int port;
            string certificateFilePath = args[1];
            if (int.TryParse(args[0], out port) && !string.IsNullOrEmpty(certificateFilePath))
            {
                _logger.InfoFormat("Listening port '{0}', Certificate file: '{1}'.",
                    port, certificateFilePath);

                var source = new CancellationTokenSource();
                service = new SecurityAsyncService(Convert.ToInt32(args[0]), args[1], source.Token);
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
            _logger.Info("ServerConsole [port] [X509Certificate file path]");
        }

        private static void Quit()
        {
            const string keyword = "quit";
            Console.WriteLine("Type '{0}' to stop.", keyword);
            string input = Console.ReadLine();
            if (!string.Equals(keyword, input, StringComparison.InvariantCultureIgnoreCase))
            {
                Quit();
            }
        }
    }
}