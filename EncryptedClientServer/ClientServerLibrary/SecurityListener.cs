using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ClientServerLibrary
{
    public class SecurityListener : NetworkCommon
    {
        public static readonly int DefaultBufferSize = 8 * 1024;
        /// <summary>
        /// Default length of pending connections queue, value: 1.
        /// </summary>
        public static readonly int DefaultBacklog = 10;

        private readonly int _backlog;
        private readonly int _bufferSize;

        /// <summary>
        /// Key: Socket handle
        /// </summary>
        private ConcurrentDictionary<IntPtr, SocketAsyncEventArgs> _acceptSocketMapping;

        public SecurityListener() : this(AddressFamily.InterNetwork, DefaultPort, DefaultBacklog, DefaultBufferSize) {}

        public SecurityListener(AddressFamily family, int port, int backlog, int bufferSize) : base(family, port)
        {
            _backlog = backlog;
            _bufferSize = bufferSize;

            _acceptSocketMapping = new ConcurrentDictionary<IntPtr, SocketAsyncEventArgs>();
        }

        public void StartListen()
        {
            Logger.Info("Starting service...");

            IPEndPoint endPoint = GetLocalEndPoint();
            NetworkSocket.Bind(endPoint);
            NetworkSocket.Listen(_backlog);

            StartAccept();
        }

        // Handle socket accept
        private void StartAccept()
        {
            Logger.Info("Star to accept socket connection.");

            SocketAsyncEventArgs acceptArgs = new SocketAsyncEventArgs();
            acceptArgs.Completed += AcceptCompleted;

            bool willRaiseEvent = NetworkSocket.AcceptAsync(acceptArgs);
            Logger.InfoFormat("Accepted socket connection, rise async event? {0}.", willRaiseEvent);

            if (!willRaiseEvent)
            {
                Logger.Debug("The socket connect operation completed synchronously");
                AcceptCompleted(NetworkSocket, acceptArgs);
            }
        }

        // TODO: To static?
        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.AcceptSocket != null)
            {
                IntPtr handle = e.AcceptSocket.Handle;
                if (_acceptSocketMapping.TryAdd(handle, e))
                {
                    Logger.InfoFormat("Accepted the socket handle: {0}.", handle);
                    StartReceive(e);
                    StartAccept();
                }
                else
                {
                    StartAccept();

                    Logger.WarnFormat("The socket handle already exists, value: {0}.", handle);
                    ShutdownSocket(e.AcceptSocket);
                }
            }
            else
            {
                StartAccept();

                Logger.WarnFormat("Cannot accept socket connection, error code: {0}, socket handle: {1}.",
                    e.SocketError, e != null && e.AcceptSocket != null ? e.AcceptSocket.Handle : IntPtr.Zero);
                ShutdownSocket(e.AcceptSocket);
            }
        }

        private void StartReceive(SocketAsyncEventArgs eventArgs)
        {
            eventArgs.Completed -= AcceptCompleted;
            eventArgs.Completed += IOCompleted;

            byte[] buffer = new byte[_bufferSize];
            eventArgs.SetBuffer(buffer, 0, _bufferSize);

            if (!eventArgs.AcceptSocket.ReceiveAsync(eventArgs))
            {
                Logger.Debug("The I/O operation completed synchronously");
                HandleTransferException(() => ProcessReceive(eventArgs), eventArgs.AcceptSocket);
            }
        }

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            Logger.DebugFormat("IO operation completed, last operation: {0}.", e.LastOperation);
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    HandleTransferException(() => ProcessReceive(e), e.AcceptSocket);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Not support operation type: {0}.", e.LastOperation));
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            Logger.DebugFormat("Received data size: {0}.", e.BytesTransferred);

            string receivedMessage = System.Text.Encoding.Unicode.GetString(e.Buffer, 0, e.BytesTransferred);
            Logger.InfoFormat("Received message from client: {0}. Sending data back.", receivedMessage);

            string responseMessage = string.Format("Received messag from client: {0}.", receivedMessage);
            byte[] responseBuffer = System.Text.Encoding.Unicode.GetBytes(responseMessage);

            e.AcceptSocket.Send(responseBuffer);

            if (!e.AcceptSocket.ReceiveAsync(e))
            {
                Logger.Debug("The I/O operation completed synchronously");
                ProcessReceive(e);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {

        }

        private IPEndPoint GetLocalEndPoint()
        {
            if (_family != AddressFamily.InterNetwork &&  _family != AddressFamily.InterNetworkV6)
            {
                throw new NotSupportedException(string.Format("Address family '{0}' Not supported.", _family));
            }

            return _family == AddressFamily.InterNetworkV6 ?
                new IPEndPoint(IPAddress.IPv6Any, _port) : new IPEndPoint(IPAddress.Any, _port);
        }

        protected override void OnClosedAcceptSocket(IntPtr handle)
        {
            SocketAsyncEventArgs removedArgs;
            if (!_acceptSocketMapping.TryRemove(handle, out removedArgs))
            {
                Logger.ErrorFormat("Cannot remove the accepted socket, handle :{0}.", handle);
            }
        }
    }
}