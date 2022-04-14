using System;
using System.IO;

namespace fNbt.Tests;

internal class PartialReadStream : Stream
{
    private readonly Stream baseStream;
    private readonly int increment;

    public PartialReadStream([NotNull] Stream baseStream)
        : this(baseStream, 1)
    {
    }


    public PartialReadStream([NotNull] Stream baseStream, int increment)
    {
        if (baseStream == null)
        {
            throw new ArgumentNullException(nameof(baseStream));
        }

        this.baseStream = baseStream;
        this.increment = increment;
    }


    public override bool CanRead => true;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => false;

    public override long Length => baseStream.Length;

    public override long Position
    {
        get => baseStream.Position;
        set => baseStream.Position = value;
    }


    public override void Flush()
    {
        baseStream.Flush();
    }


    public override long Seek(long offset, SeekOrigin origin)
    {
        return baseStream.Seek(offset, origin);
    }


    public override void SetLength(long value)
    {
        baseStream.SetLength(value);
    }


    public override int Read(byte[] buffer, int offset, int count)
    {
        var bytesToRead = Math.Min(increment, count);
        return baseStream.Read(buffer, offset, bytesToRead);
    }


    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }
}