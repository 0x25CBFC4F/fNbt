﻿using System.Diagnostics;

namespace fNbt;

// Class used to count bytes read-from/written-to non-seekable streams.
internal class ByteCountingStream : Stream
{
    private readonly Stream baseStream;

    // These are necessary to avoid counting bytes twice if Read/Write call ReadByte/WriteByte internally.
    private bool readingManyBytes;

    // These are necessary to avoid counting bytes twice if ReadByte/WriteByte call Read/Write internally.
    private bool readingOneByte;
    private bool writingManyBytes;
    private bool writingOneByte;


    public ByteCountingStream([NotNull] Stream stream)
    {
        Debug.Assert(stream != null);
        baseStream = stream;
    }


    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position
    {
        get => baseStream.Position;
        set => baseStream.Position = value;
    }

    public long BytesRead { get; private set; }
    public long BytesWritten { get; private set; }


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
        readingManyBytes = true;
        var bytesActuallyRead = baseStream.Read(buffer, offset, count);
        readingManyBytes = false;
        if (!readingOneByte)
        {
            BytesRead += bytesActuallyRead;
        }

        return bytesActuallyRead;
    }


    public override void Write(byte[] buffer, int offset, int count)
    {
        writingManyBytes = true;
        baseStream.Write(buffer, offset, count);
        writingManyBytes = false;
        if (!writingOneByte)
        {
            BytesWritten += count;
        }
    }


    public override int ReadByte()
    {
        readingOneByte = true;
        var value = base.ReadByte();
        readingOneByte = false;
        if (value >= 0 && !readingManyBytes)
        {
            BytesRead++;
        }

        return value;
    }


    public override void WriteByte(byte value)
    {
        writingOneByte = true;
        base.WriteByte(value);
        writingOneByte = false;
        if (!writingManyBytes)
        {
            BytesWritten++;
        }
    }
}