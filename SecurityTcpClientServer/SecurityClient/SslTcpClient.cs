using System;
using System.Net.Sockets;

namespace SecurityClient
{
    public class SslTcpClient : TcpClient
    {
        private string _serverCertificateName;

        public SslTcpClient(string machineName, int port, string serverCertificateName) : base(machineName, port)
        {
            _serverCertificateName = serverCertificateName;
        }

        //public void 
    }
}
