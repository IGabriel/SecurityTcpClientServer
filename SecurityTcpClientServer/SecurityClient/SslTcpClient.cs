using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Common;

namespace SecurityClient
{
    public class SslTcpClient : DisposableObject
    {
        private string _machineName;
        private int _port;
        private string _serverCertificateName;

        private TcpClient _client;

        public SslTcpClient(string machineName, int port, string serverCertificateName)
        {
            _machineName = machineName;
            _port = port;
            _serverCertificateName = serverCertificateName;
        }

        public void Connect()
        {
            Logger.InfoFormat("Connecting to remote server: {0} with port: {1}.", _machineName, _port);

            _client = new TcpClient(_machineName, _port);
            using (SslStream stream = new SslStream(_client.GetStream(), false, ValidateServerCertificate, null))
            {
                stream.AuthenticateAsClient(_machineName);

                byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
                stream.Write(messsage);
                stream.Flush();
                // Read message from the server.
                // string serverMessage = ReadMessage(stream);
                // Console.WriteLine("Server says: {0}", serverMessage);
                // Close the client connection.
                stream.Close();
                Logger.Info("Client closed.");
            }
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool result = sslPolicyErrors == SslPolicyErrors.None;
            Logger.InfoFormat("Certificate error: {0}, result: {1}.", sslPolicyErrors, result);
            
            return result;
        }
        
        protected override void DisposeResource()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}
