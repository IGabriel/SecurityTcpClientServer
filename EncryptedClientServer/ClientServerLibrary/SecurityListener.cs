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
        public static readonly int DefaultBufferSize;

        static SecurityListener()
        {
            // DefaultBufferSize = 8 * 1024;
            DefaultBufferSize = 100;
        }

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
                using (stream)
                {
                    byte[] buffer = new byte[DefaultBufferSize];

                    int readCount = stream.Read(buffer, 0, DefaultBufferSize);

                    Logger.InfoFormat("Received data., count: {0}, content: {1}.", readCount, System.BitConverter.ToString(buffer));

                    Logger.Info("Sending data back.");

                    const string responseMessage = "Server send message back.";
                    byte[] responseBuffer = System.Text.Encoding.Unicode.GetBytes(responseMessage);

                    Logger.InfoFormat("Sending data., count: {0}, content: {1}.", responseBuffer.Length, System.BitConverter.ToString(responseBuffer));

                    await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);



                }
            }
        }
    }
}