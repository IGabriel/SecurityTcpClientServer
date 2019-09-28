using System;
using System.Collections.Concurrent;

namespace ClientServerLibrary
{
    public class DuplicatedClientConnectionException : Exception
    {
        public IntPtr SocketHandle { get; }
        public ConcurrentDictionary<IntPtr, AsyncState> Mapping { get; }

        public DuplicatedClientConnectionException(IntPtr handle, ConcurrentDictionary<IntPtr, AsyncState> mapping) : base()
        {
            SocketHandle = handle;
            Mapping = mapping;
        }
    }
}