namespace Newtonsoft.Json.Utilities
{
    using Newtonsoft.Json;
    using System;

    internal static class BufferUtils
    {
        public static char[] EnsureBufferSize(IArrayPool<char> bufferPool, int size, char[] buffer)
        {
            if (bufferPool == null)
            {
                return new char[size];
            }
            if (buffer != null)
            {
                bufferPool.Return(buffer);
            }
            return bufferPool.Rent(size);
        }

        public static char[] RentBuffer(IArrayPool<char> bufferPool, int minSize) => 
            bufferPool?.Rent(minSize);

        public static void ReturnBuffer(IArrayPool<char> bufferPool, char[] buffer)
        {
            if (bufferPool != null)
            {
                bufferPool.Return(buffer);
            }
        }
    }
}

