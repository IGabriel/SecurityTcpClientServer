using System;
using System.Net.Sockets;

namespace ClientServerLibrary
{
    public abstract class NetworkCommon : LogObject
    {
        public static readonly int DefaultPort = 7788;

        private Socket _socket;

        protected readonly string _machineName;
        protected readonly int _port;
        protected readonly AddressFamily _family;

        protected Socket NetworkSocket
        {
            get
            {
                if (_socket == null)
                {
                    _socket = new Socket(_family, SocketType.Stream, ProtocolType.Tcp);
                }
                return _socket;
            }
        }

        public NetworkCommon(AddressFamily family, int port)
        {
            _family = family;
            _port = port;
        }

        public NetworkCommon(AddressFamily family) : this(family, DefaultPort) {}

        protected void HandleTransferException(Action transferAction, Socket currentSocket)
        {
            try
            {
                transferAction();
            }
            catch (System.Exception ex)
            {
                Logger.Error(string.Format("Rise exception is method: {0}", transferAction.Method.Name), ex);
                if (currentSocket != null)
                {
                    ShutdownSocket(currentSocket);
                }
                else
                {
                    Logger.Warn("No accept socket.");
                }
            }
        }

        public void ShutdownSocket(Socket socket)
        {
            ShutdownSocket(socket, string.Empty);
        }

        /// <summary>
        /// Shutdown socket.
        /// </summary>
        /// <param name="socket">connected socket</param>
        /// <param name="reason">Text to describe the reason, message format: 'Shutting down socket '{0}', reason: {1}.'</param>
        public void ShutdownSocket(Socket socket, string reason)
        {
            if (socket == null)
            {
                Logger.Error("Target socket is null.");
                return;
            }

            Logger.InfoFormat("Shutting down socket '{0}', reason: {1}.", socket.Handle, reason);

            IntPtr handleValue = socket.Handle;
            OnClosingSocket(handleValue);

            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Cannot shutdown and close the socket, handle: {0}.", socket.Handle), ex);
            }

            OnClosedSocket(handleValue);
        }

        protected virtual void OnClosingSocket(IntPtr handle) {}
        protected virtual void OnClosedSocket(IntPtr handle) {}

    }
}