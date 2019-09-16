using System;
using System.Net.Sockets;
using System.Text;

using Common;

namespace EncryptedClient
{
    public class SecurityClient : DisposableObject
    {
        private string _machineName;
        private int _port;
        private TcpClient _client;

        public SecurityClient(string machineName, int port)
        {
            _machineName = machineName;
            _port = port;
        }

        public void Connect()
        {
            Logger.InfoFormat("Connecting to remote server: {0} with port: {1}.", _machineName, _port);

            _client = new TcpClient(_machineName, _port);
            NetworkStream stream = _client.GetStream();
            using (stream)
            {
                byte[] messsage = Encoding.UTF8.GetBytes("Hello from the client.<EOF>");
                stream.Write(messsage, 0, messsage.Length);
                stream.Flush();
                // Read message from the server.
                // string serverMessage = ReadMessage(stream);
                // Console.WriteLine("Server says: {0}", serverMessage);
                // Close the client connection.
                stream.Close();
                Logger.Info("Client closed.");
            }
        }

        protected override void DisposeResource()
        {
            if (_client != null)
            {
                try
                {
                    _client.Close();
                }
                catch
                {

                }
                _client.Dispose();
            }
        }
    }
}
