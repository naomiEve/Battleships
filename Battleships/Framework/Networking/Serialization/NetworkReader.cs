using System.Runtime.CompilerServices;
using System.Text;

namespace Battleships.Framework.Networking.Serialization
{
    /// <summary>
    /// A fast stack-allocated network reader.
    /// </summary>
    internal ref struct NetworkReader
    {
        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly ReadOnlySpan<byte> _buffer;

        /// <summary>
        /// The current position of the reader.
        /// </summary>
        private int _position;

        /// <summary>
        /// How many bytes we've read so far.
        /// </summary>
        public int Position => _position;

        /// <summary>
        /// Constructs a new network reader with the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public NetworkReader(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
            _position = 0;
        }

        /// <summary>
        /// Reads an unmanaged (non-CLR) data type from the buffer.
        /// </summary>
        /// <typeparam name="TUnmanaged">The unmanaged data type.</typeparam>
        public unsafe TUnmanaged Read<TUnmanaged>()
            where TUnmanaged : unmanaged
        {
            var data = default(TUnmanaged);
            fixed (byte* bufferPtr = _buffer)
                data = Unsafe.Read<TUnmanaged>(bufferPtr + _position);

            _position += sizeof(TUnmanaged);
            return data;
        }

        /// <summary>
        /// Reads count bytes from the stream.
        /// </summary>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>A ReadOnlySpan view of the data.</returns>
        public ReadOnlySpan<byte> ReadBytes(int count)
        {
            var slice = _buffer.Slice(_position, count);
            _position += count;

            return slice;
        }

        /// <summary>
        /// Reads a string from the stream.
        /// </summary>
        /// <returns>The string.</returns>
        public string ReadString()
        {
            var length = Read<int>();
            return Encoding.UTF8.GetString(ReadBytes(length));
        }
    }
}
