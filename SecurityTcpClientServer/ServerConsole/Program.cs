using System;
using SecurityServer;

namespace ServerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            SecurityAsyncService service;
            if (args.Length == 0)
            {
                service = new SecurityAsyncService();
            }
            else if (args.Length == 1)
            {
                service = new SecurityAsyncService(Convert.ToInt32(args[0]));
            }
            else
            {
                Console.WriteLine("Listen the TCP port.");
                Console.WriteLine("ServerConsole <[port]>");
            }
        }
    }
}
