using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
            SendMessage(stream, "The first message!");
            SendMessage(stream, "The second message!");
            SendMessage(stream, "The thrid message!");
        }

        private void SendMessage(NetworkStream stream, string message)
        {
            Thread.Sleep(1000);

            byte[] messsageBuffer = Encoding.Unicode.GetBytes(message);

            stream.Write(messsageBuffer, 0, messsageBuffer.Length);

            stream.Flush();

            Logger.InfoFormat("Sent message to server: {0}.", message);

            byte[] receiveBuffer = new byte[1024];
            int count = stream.Read(receiveBuffer, 0, 100);

            stream.Flush();


            string messageFromServer1 = Encoding.Unicode.GetString(receiveBuffer, 0, count);
            Logger.DebugFormat("Message from server: {0}", messageFromServer1);
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