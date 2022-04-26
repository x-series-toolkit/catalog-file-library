#if NETSTANDARD2_0 || NET461
using System.Buffers;

namespace X4.CatalogFileLib.Extensions;

public static class StreamExtensions
{
    public static int Read(this Stream thisStream, Span<byte> buffer)
    {
        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            int numRead = thisStream.Read(sharedBuffer, 0, buffer.Length);
            if ((uint)numRead > (uint)buffer.Length)
            {
                throw new IOException("SR.IO_StreamTooLong");
            }
            new Span<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
            return numRead;
        }
        finally { ArrayPool<byte>.Shared.Return(sharedBuffer); }
    }

    public static void Write(this Stream thisStream, ReadOnlySpan<byte> buffer)
    {
        byte[] sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            buffer.CopyTo(sharedBuffer);
            thisStream.Write(sharedBuffer, 0, buffer.Length);
        }
        finally { ArrayPool<byte>.Shared.Return(sharedBuffer); }
    }
}
#endif