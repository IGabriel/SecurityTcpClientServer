using System;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace ClientServerLibrary
{
    public class DuplicatedClientConnectionException : Exception
    {
        public IntPtr SocketHandle { get; }
        public ConcurrentDictionary<IntPtr, SocketAsyncEventArgs> Mapping { get; }

        public DuplicatedClientConnectionException(IntPtr handle, ConcurrentDictionary<IntPtr, SocketAsyncEventArgs> mapping) : base()
        {
            SocketHandle = handle;
            Mapping = mapping;
        }
    }
}