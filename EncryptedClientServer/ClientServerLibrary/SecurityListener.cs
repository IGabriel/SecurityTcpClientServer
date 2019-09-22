using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServerLibrary
{
    public class SecurityListener : LogObject
    {
        private TcpListener _listener;
        private int _port;

        public CancellationToken Token {get; }
        public TcpListener Listener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = new TcpListener(IPAddress.Any, _port);
                }
                return _listener;
            }
        }


        public SecurityListener(int port, CancellationToken token)
        {
            _port = port;
            Token = token;
        }

        public async void Start()
        {
            Logger.Info("Starting Async service...");

            Listener.Start();
            while(!Token.IsCancellationRequested)
            {
                try
                {
                    await Listener.AcceptTcpClientAsync().ContinueWith(Process);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Unexpected exception, message: {0}.", ex.Message);
                    // TODO: Logging, close connection and release resource.
                }
            }

            Logger.Info("Stopping Async service...");
            Listener.Stop();
        }

        private async Task Process(Task<TcpClient> clientTask)
        {
            TcpClient client = await clientTask;
            using (client)
            {
                NetworkStream stream = client.GetStream();
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    string message = reader.ReadString();
                    Logger.InfoFormat("Received data: {0}.", message);
                }
            }
        }
    }
}