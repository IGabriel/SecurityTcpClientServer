using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SecurityServer;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
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
            Console.WriteLine("Listen the TCP port:");
            Console.WriteLine("ServerConsole [port] [X509Certificate file path]");
        }

        private static void Quit()
        {
            const string keyword = "quie";
            Console.WriteLine("Type '{0}' to stop.", keyword);
            string input = Console.ReadLine();
            if (!string.Equals(keyword, input, StringComparison.InvariantCultureIgnoreCase))
            {
                Quit();
            }
        }
    }
}
