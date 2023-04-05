using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Battleships.Framework.Networking.Serialization
{
    /// <summary>
    /// A fast stack-allocated network writer.
    /// </summary>
    internal ref struct NetworkWriter
    {
        /// <summary>
        /// The buffer we're writing to.
        /// </summary>
        private readonly Span<byte> _buffer;

        /// <summary>
        /// Current position
        /// </summary>
        private int _position;

        /// <summary>
        /// The amount of data written.
        /// </summary>
        public int Written => _position;

        /// <summary>
        /// Constructs a new network writer with the given buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public NetworkWriter(Span<byte> buffer)
        {
            _buffer = buffer;
        }

        /// <summary>
        /// Writes an unmanaged (non-CLR) data type into the buffer.
        /// </summary>
        /// <typeparam name="TUnmanaged">The unmanaged data type.</typeparam>
        /// <param name="data">The data to write.</param>
        public unsafe void Write<TUnmanaged>(TUnmanaged data)
            where TUnmanaged : unmanaged
        {
            fixed (byte* bufferPtr = _buffer)
                Unsafe.Write(bufferPtr + _position, data);
            
            _position += sizeof(TUnmanaged);
        }

        /// <summary>
        /// Writes a string into the buffer. Allocates, so be wary.
        /// </summary>
        /// <param name="str"></param>
        public void WriteString(string str)
        {
            Write(str.Length);

            var data = Encoding.UTF8.GetBytes(str);
            WriteBytes(data);
        }

        /// <summary>
        /// Writes a span of bytes into the buffer.
        /// </summary>
        /// <param name="bytes">The span of bytes.</param>
        public void WriteBytes(ReadOnlySpan<byte> bytes)
        {
            for (var i = 0; i < bytes.Length; i++)
                Write(bytes[i]);
        }
    }
}
