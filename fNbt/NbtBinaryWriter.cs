using System.Text;

namespace fNbt;

/// <summary>
///     BinaryWriter wrapper that writes NBT primitives to a stream,
///     while taking care of endianness and string encoding, and counting bytes written.
/// </summary>
internal sealed unsafe class NbtBinaryWriter
{
    // Write at most 4 MiB at a time.
    public const int MaxWriteChunk = 4 * 1024 * 1024;

    // Buffer used for temporary conversion
    private const int BufferSize = 256;

    // UTF8 characters use at most 4 bytes each.
    private const int MaxBufferedStringLength = BufferSize / 4;

    // Encoding can be shared among all instances of NbtBinaryWriter, because it is stateless.
    private static readonly UTF8Encoding Encoding = new(false, true);

    // Each NbtBinaryWriter needs to have its own instance of the buffer.
    private readonly byte[] buffer = new byte[BufferSize];

    // Each instance has to have its own encoder, because it does maintain state.
    private readonly Encoder encoder = Encoding.GetEncoder();

    private readonly Stream stream;

    // Swap is only needed if endianness of the runtime differs from desired NBT stream
    private readonly bool swapNeeded;


    public NbtBinaryWriter([NotNull] Stream input, bool bigEndian)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (!input.CanWrite)
        {
            throw new ArgumentException("Given stream must be writable", nameof(input));
        }

        stream = input;
        swapNeeded = BitConverter.IsLittleEndian == bigEndian;
    }

    public Stream BaseStream
    {
        get
        {
            stream.Flush();
            return stream;
        }
    }


    public void Write(byte value)
    {
        stream.WriteByte(value);
    }


    public void Write(NbtTagType value)
    {
        stream.WriteByte((byte)value);
    }


    public void Write(short value)
    {
        unchecked
        {
            if (swapNeeded)
            {
                buffer[0] = (byte)(value >> 8);
                buffer[1] = (byte)value;
            }
            else
            {
                buffer[0] = (byte)value;
                buffer[1] = (byte)(value >> 8);
            }
        }

        stream.Write(buffer, 0, 2);
    }


    public void Write(int value)
    {
        unchecked
        {
            if (swapNeeded)
            {
                buffer[0] = (byte)(value >> 24);
                buffer[1] = (byte)(value >> 16);
                buffer[2] = (byte)(value >> 8);
                buffer[3] = (byte)value;
            }
            else
            {
                buffer[0] = (byte)value;
                buffer[1] = (byte)(value >> 8);
                buffer[2] = (byte)(value >> 16);
                buffer[3] = (byte)(value >> 24);
            }
        }

        stream.Write(buffer, 0, 4);
    }


    public void Write(long value)
    {
        unchecked
        {
            if (swapNeeded)
            {
                buffer[0] = (byte)(value >> 56);
                buffer[1] = (byte)(value >> 48);
                buffer[2] = (byte)(value >> 40);
                buffer[3] = (byte)(value >> 32);
                buffer[4] = (byte)(value >> 24);
                buffer[5] = (byte)(value >> 16);
                buffer[6] = (byte)(value >> 8);
                buffer[7] = (byte)value;
            }
            else
            {
                buffer[0] = (byte)value;
                buffer[1] = (byte)(value >> 8);
                buffer[2] = (byte)(value >> 16);
                buffer[3] = (byte)(value >> 24);
                buffer[4] = (byte)(value >> 32);
                buffer[5] = (byte)(value >> 40);
                buffer[6] = (byte)(value >> 48);
                buffer[7] = (byte)(value >> 56);
            }
        }

        stream.Write(buffer, 0, 8);
    }


    public void Write(float value)
    {
        ulong tmpValue = *(uint*)&value;
        unchecked
        {
            if (swapNeeded)
            {
                buffer[0] = (byte)(tmpValue >> 24);
                buffer[1] = (byte)(tmpValue >> 16);
                buffer[2] = (byte)(tmpValue >> 8);
                buffer[3] = (byte)tmpValue;
            }
            else
            {
                buffer[0] = (byte)tmpValue;
                buffer[1] = (byte)(tmpValue >> 8);
                buffer[2] = (byte)(tmpValue >> 16);
                buffer[3] = (byte)(tmpValue >> 24);
            }
        }

        stream.Write(buffer, 0, 4);
    }


    public void Write(double value)
    {
        var tmpValue = *(ulong*)&value;
        unchecked
        {
            if (swapNeeded)
            {
                buffer[0] = (byte)(tmpValue >> 56);
                buffer[1] = (byte)(tmpValue >> 48);
                buffer[2] = (byte)(tmpValue >> 40);
                buffer[3] = (byte)(tmpValue >> 32);
                buffer[4] = (byte)(tmpValue >> 24);
                buffer[5] = (byte)(tmpValue >> 16);
                buffer[6] = (byte)(tmpValue >> 8);
                buffer[7] = (byte)tmpValue;
            }
            else
            {
                buffer[0] = (byte)tmpValue;
                buffer[1] = (byte)(tmpValue >> 8);
                buffer[2] = (byte)(tmpValue >> 16);
                buffer[3] = (byte)(tmpValue >> 24);
                buffer[4] = (byte)(tmpValue >> 32);
                buffer[5] = (byte)(tmpValue >> 40);
                buffer[6] = (byte)(tmpValue >> 48);
                buffer[7] = (byte)(tmpValue >> 56);
            }
        }

        stream.Write(buffer, 0, 8);
    }


    // Based on BinaryWriter.Write(String)
    public void Write([NotNull] string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // Write out string length (as number of bytes)
        var numBytes = Encoding.GetByteCount(value);
        Write((short)numBytes);

        if (numBytes <= BufferSize)
        {
            // If the string fits entirely in the buffer, encode and write it as one
            Encoding.GetBytes(value, 0, value.Length, buffer, 0);
            stream.Write(buffer, 0, numBytes);
        }
        else
        {
            // Aggressively try to not allocate memory in this loop for runtime performance reasons.
            // Use an Encoder to write out the string correctly (handling surrogates crossing buffer
            // boundaries properly).  
            var charStart = 0;
            var numLeft = value.Length;
            while (numLeft > 0)
            {
                // Figure out how many chars to process this round.
                var charCount = numLeft > MaxBufferedStringLength ? MaxBufferedStringLength : numLeft;
                int byteLen;
                fixed (char* pChars = value)
                {
                    fixed (byte* pBytes = buffer)
                    {
                        byteLen = encoder.GetBytes(pChars + charStart, charCount, pBytes, BufferSize,
                            charCount == numLeft);
                    }
                }

                stream.Write(buffer, 0, byteLen);
                charStart += charCount;
                numLeft -= charCount;
            }
        }
    }


    public void Write(byte[] data, int offset, int count)
    {
        var written = 0;
        while (written < count)
        {
            var toWrite = Math.Min(MaxWriteChunk, count - written);
            stream.Write(data, offset + written, toWrite);
            written += toWrite;
        }
    }
}