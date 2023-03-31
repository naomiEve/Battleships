using System.Buffers;

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
        private readonly Memory<byte> _buffer;

        /// <summary>
        /// The buffer we're writing to.
        /// </summary>
        private readonly MemoryHandle _bufferHandle;

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
        public NetworkWriter(Memory<byte> buffer)
        {
            _buffer = buffer;
            _bufferHandle = buffer.Pin();
        }

        /// <summary>
        /// Writes an unmanaged (non-CLR) data type into the buffer.
        /// </summary>
        /// <typeparam name="TUnmanaged">The unmanaged data type.</typeparam>
        /// <param name="data">The data to write.</param>
        public unsafe void Write<TUnmanaged>(TUnmanaged data)
            where TUnmanaged : unmanaged
        {
            *((TUnmanaged*)_bufferHandle.Pointer + _position) = data;

            _position += sizeof(TUnmanaged);
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

        /// <summary>
        /// Frees the associated buffer handle.
        /// </summary>
        public void Free()
        {
            _bufferHandle.Dispose();
        }
    }
}
