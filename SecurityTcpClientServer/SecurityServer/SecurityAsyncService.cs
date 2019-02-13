using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SecurityServer
{
    // TODO: interface?
    // TOD0: support cancel?
    // TODO: Idisposeable

    public class SecurityAsyncService
    {
        private TcpListener _listener;

        private X509Certificate _certificate;

        public string CertificateFilePath { get; }

        public int Port { get; }

        public SecurityAsyncService(int port, string certificateFilePath)
        {
            Port = port;
            CertificateFilePath = certificateFilePath;
        }

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
            _certificate = X509Certificate.CreateFromCertFile(CertificateFilePath);

            Listener.Start();
            while(true)
            {
                try
                {
                    // TODO: Use ContinueWith?
                    await Listener.AcceptTcpClientAsync().ContinueWith(Process);
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

        // private async Task<TcpClient> HandleClientAcceptedAsync()
        // {
        //     var client = await Listener.AcceptTcpClientAsync();
        //     OnClientAccepted(client);
        //     return client;
        // }

        private async Task Process(Task<TcpClient> clientTask)
        {
            TcpClient client = await clientTask;
            using(SslStream securityStream = new SslStream(client.GetStream()))
            {
                await securityStream.AuthenticateAsServerAsync(_certificate);
                using(BinaryReader reader = new BinaryReader(securityStream))
                {
                }
            }

        }

        // private async Task HandleAcceptClientAsync(Task<TcpClient> clientTask)
        // {
        //     TcpClient client = await clientTask;

        // }

        #region virtual methods
        // protected virtual void OnClientAccepted(TcpClient client)
        // {

        // }
        #endregion virtual methods
    }
}
