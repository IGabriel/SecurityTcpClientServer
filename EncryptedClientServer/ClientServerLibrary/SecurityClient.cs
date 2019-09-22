using System;
using System.Net.Sockets;
using System.Text;

namespace ClientServerLibrary
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
                const string message = "Hello from the client.<EOF>";
                byte[] messsageBuffer = Encoding.Unicode.GetBytes(message);

                Logger.DebugFormat("Sending a text: '{0}', buffer Length: {1}, content: {2}",
                    message, messsageBuffer.Length, BitConverter.ToString(messsageBuffer));

                stream.Write(messsageBuffer, 0, messsageBuffer.Length);
                stream.Flush();
                // Read message from the server.
                // string serverMessage = ReadMessage(stream);
                // Console.WriteLine("Server says: {0}", serverMessage);
                // Close the client connection.
                stream.Close();

                Logger.Info("Message sent.");
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