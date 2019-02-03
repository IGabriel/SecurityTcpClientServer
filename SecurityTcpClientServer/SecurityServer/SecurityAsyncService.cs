using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SecurityServer
{
    // TODO: interface?
    // TOD0: support cancel?

    public class SecurityAsyncService
    {
        private TcpListener _listener;

        public static readonly int DefaultPort = 7788;
        public SecurityAsyncService(int port) 
        {
            Port = port;               
        }

        public SecurityAsyncService() : this(DefaultPort) {}

        public int Port { get; }

        public TcpListener Listener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = new TcpListener(IPAddress.Any, Port);
                }
                return _listener;
            }
        }

        public async void Run()
        {
            Listener.Start();

            while(true)
            {
                try
                {
                    // TODO: Use ContinueWith?
                    await Listener.AcceptTcpClientAsync().ContinueWith(HandleAcceptClientAsync);
                }
                catch
                {
                    // TODO: Logging, close connection and release resource.
                }
            }
        }

        public void Stop()
        {
            Listener.Stop();
        }

        private async Task HandleAcceptClientAsync(Task<TcpClient> client)
        {

        }
    }
}
