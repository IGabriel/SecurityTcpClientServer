using System.Net.Sockets;

namespace ClientServerLibrary
{
    public class AsyncState : SocketAsyncEventArgs
    {
        /// <summary>
        /// Default receive / send buffer size: 8K.
        /// </summary>
        public static readonly int DefaultBufferSize = 8 * 1024;

        public int ReceivedSize { get; set; }

        public AsyncState(Socket acceptSocket) : this(acceptSocket, DefaultBufferSize) {}

        public AsyncState(Socket acceptSocket, int bufferSize)
        {
            AcceptSocket = acceptSocket;
        }
    }
}