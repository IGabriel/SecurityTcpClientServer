using System;
using System.IO;
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
                service = new SecurityAsyncService(Convert.ToInt32(args[0]), args[1]);
                service.Run();
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
    }
}
