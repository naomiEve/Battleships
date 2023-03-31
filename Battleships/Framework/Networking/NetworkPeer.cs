using System;
using Battleships.Framework.Networking.Messages;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// An abstract lockstep network peer, common class for both the sevrer and client.
    /// </summary>
    internal abstract class NetworkPeer
    {
        /// <summary>
        /// The maximum buffer size. (1MiB)
        /// </summary>
        private const int MAX_BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        /// The map from message numbers to their types. 
        /// </summary>
        private readonly Dictionary<int, Type> _messageTypeMap;

        /// <summary>
        /// The send/receive buffer.
        /// </summary>
        private readonly Memory<byte> _buffer;

        /// <summary>
        /// Is this peer the current peer that's being waited on by the lockstep simulation?
        /// </summary>
        public bool IsCurrentLockstepPeer { get; protected set; } = false;

        /// <summary>
        /// Constructs a new network peer.
        /// </summary>
        public NetworkPeer()
        {
            _messageTypeMap = new Dictionary<int, Type>();
            _buffer = new Memory<byte>(new byte[MAX_BUFFER_SIZE]);

            RegisterMessage<LockstepPassingMessage>();
        }

        /// <summary>
        /// Is the peer ready?
        /// </summary>
        public abstract bool Ready { get; }

        /// <summary>
        /// Waits until this peer is ready to work.
        /// </summary>
        public abstract void WaitUntilReady();

        /// <summary>
        /// Sends bytes to the other peer.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        protected abstract void SendBytes(ReadOnlySpan<byte> bytes);

        /// <summary>
        /// Receives bytes from the other party.
        /// </summary>
        /// <param name="memory">The data to receive.</param>
        protected abstract int ReceiveBytes(Memory<byte> memory);

        /// <summary>
        /// Register a network message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        public void RegisterMessage<TMessage>()
            where TMessage : INetworkMessage
        {
            _messageTypeMap.Add(typeof(TMessage).GetHashCode(), typeof(TMessage));
            Console.WriteLine($"Registered {typeof(TMessage).FullName}=>{typeof(TMessage).GetHashCode()}");
        }

        /// <summary>
        /// Send a network message.
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        /// <param name="mode"></param>
        public void Send<TMessage>(TMessage message, SendMode mode = SendMode.Lockstep)
            where TMessage : INetworkMessage
        {
            if (mode == SendMode.Lockstep)
            {
                if (!IsCurrentLockstepPeer)
                    return;
            }

            var id = typeof(TMessage).GetHashCode();
            if (!_messageTypeMap.ContainsKey(id))
                return;

            var packet = new NetworkPacket(message, mode);

            var writer = new NetworkWriter(_buffer.Span);
            var written = packet.Serialize(ref writer);

            SendBytes(_buffer.Span[..written]);
        }

        /// <summary>
        /// Receives all the currently pending messages.
        /// </summary>
        public void Receive()
        {
            while (true)
            {
                var read = ReceiveBytes(_buffer);
                if (read == 0)
                    break;

                var packet = new NetworkPacket(new NetworkReader(_buffer.Span));
            }
        }
    }
}
