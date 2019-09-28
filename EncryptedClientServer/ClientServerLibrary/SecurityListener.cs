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
    public class SecurityListener : LogObject
    {
        public static readonly int DefaultBufferSize = 8 * 1024;
        /// <summary>
        /// Default listening port, value: 7788
        /// </summary>
        public static readonly int DefaultPort = 7788;

        /// <summary>
        /// Default length of pending connections queue, value: 1.
        /// </summary>
        public static readonly int DefaultBacklog = 10;

        private Socket _listener;
        private readonly AddressFamily _family;
        private readonly int _port;
        private readonly int _backlog;
        private readonly int _bufferSize;
        private readonly SocketAsyncEventArgs _acceptEventArgs;

        /// <summary>
        /// Key: Socket handle
        /// </summary>
        //private ConcurrentDictionary<IntPtr, AsyncState> _connectedMapping;
        private ConcurrentDictionary<IntPtr, SocketAsyncEventArgs> _connectedMapping;

        private Socket Listener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = new Socket(_family, SocketType.Stream, ProtocolType.Tcp);
                }
                return _listener;
            }
        }

        public SecurityListener() : this(AddressFamily.InterNetwork, DefaultPort, DefaultBacklog, DefaultBufferSize) {}

        public SecurityListener(AddressFamily family, int port, int backlog, int bufferSize)
        {
            _family = family;
            _port = port;
            _backlog = backlog;
            _bufferSize = bufferSize;

            _acceptEventArgs = new SocketAsyncEventArgs();
            _connectedMapping = new ConcurrentDictionary<IntPtr, SocketAsyncEventArgs>();
        }

        public void Start()
        {
            Logger.Info("Starting Async service...");

            _acceptEventArgs.Completed += AcceptCompleted;

            Listener.Bind(GetLocalEndPoint());
            Listener.Listen(_backlog);

            ProcessAccept();

            Logger.Info("Stopping Async service...");

        }

        private void ProcessAccept()
        {
            SetAcceptedSocket(null);

            bool ioResult = Listener.AcceptAsync(_acceptEventArgs);
            Logger.InfoFormat("Processing client connection, IO operation result: {0}", ioResult);

            if (!ioResult)
            {
                AcceptCompleted(this, _acceptEventArgs);
            }
        }

        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            SocketError errorCode = e.SocketError;
            Logger.InfoFormat("Client connection accepted, error: {0}", errorCode);

            if (errorCode == SocketError.Success)
            {
                Socket acceptSocket = e.AcceptSocket;

                byte[] buffer = new byte[_bufferSize];
                SocketAsyncEventArgs state = new SocketAsyncEventArgs { AcceptSocket = acceptSocket };
                state.SetBuffer(buffer, 0, _bufferSize);
                state.Completed += IOCompleted;

                if (!_connectedMapping.TryAdd(acceptSocket.Handle, state))
                {
                    throw new DuplicatedClientConnectionException(e.AcceptSocket.Handle, _connectedMapping);
                }

                if (!acceptSocket.ReceiveAsync(state))
                {
                    Logger.Debug("The I/O operation completed synchronously");
                    ProcessReceive(state);
                }
            }
            ProcessAccept();
        }

        private void IOCompleted(object sender, SocketAsyncEventArgs e)
        {
            Logger.DebugFormat("IO operation completed, last operation: {0}.", e.LastOperation);
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
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

        private void SetAcceptedSocket(Socket acceptedSocket)
        {
            // BUG here!!! should lock the async operation.
            lock(_acceptEventArgs)
            {
                _acceptEventArgs.AcceptSocket = acceptedSocket;
            }
        }

        private void HandleTextMessageInternal(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            int readCount = stream.Read(buffer, 0, 1024);
            string receivedMessage = System.Text.Encoding.Unicode.GetString(buffer, 0, readCount);
            Logger.InfoFormat("Received message from client: {0}. Sending data back.", receivedMessage);

            string responseMessage = string.Format("Received messag from client: {0}.", receivedMessage);
            byte[] responseBuffer = System.Text.Encoding.Unicode.GetBytes(responseMessage);
            stream.Write(responseBuffer, 0, responseBuffer.Length);
        }
    }
}