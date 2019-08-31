using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace SecurityServer
{
    // TODO: interface?
    // TOD0: support cancel?
    // TODO: Idisposeable

    public class SecurityAsyncService : LoggedObject
    {
        private TcpListener _listener;

        private X509Certificate _certificate;

        private CancellationToken _token;

        public string CertificateFilePath { get; }

        public int Port { get; }

        public SecurityAsyncService(int port, string certificateFilePath, CancellationToken token)
        {
            Port = port;
            CertificateFilePath = certificateFilePath;
            _token = token;
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

        public async void Start()
        {
            Logger.Info("Starting Async service...");

            _certificate = X509Certificate.CreateFromCertFile(CertificateFilePath);

            Listener.Start();
            while(_token.IsCancellationRequested)
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
