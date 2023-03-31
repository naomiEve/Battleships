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
            fixed (byte* span = _buffer)
                data = *((TUnmanaged*)span + _position);

            _position += sizeof(TUnmanaged);

            return data;
        }
    }
}
