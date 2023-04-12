using Battleships.Framework.Networking.Messages;
using Battleships.Framework.Networking.Serialization;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// An abstract lockstep network peer, common class for both the sevrer and client.
    /// </summary>
    internal abstract partial class NetworkPeer
    {
        /// <summary>
        /// The maximum buffer size. (1MiB)
        /// </summary>
        private const int MAX_BUFFER_SIZE = 1024 * 1024;

        /// <summary>
        /// The send/receive buffer.
        /// </summary>
        private readonly Memory<byte> _buffer;

        /// <summary>
        /// The message registry, containing all of the packets.
        /// </summary>
        public MessageRegistry MessageRegistry { get; init; }

        /// <summary>
        /// Is this peer the current peer that's being waited on by the lockstep simulation?
        /// </summary>
        public bool IsCurrentLockstepPeer { get; protected set; } = false;

        /// <summary>
        /// This peer's id.
        /// </summary>
        public int PeerId { get; protected set; } = 0;

        /// <summary>
        /// Constructs a new network peer.
        /// </summary>
        public NetworkPeer()
        {
            _buffer = new Memory<byte>(new byte[MAX_BUFFER_SIZE]);
            MessageRegistry = new MessageRegistry();

            MessageRegistry.RegisterMessage<LockstepPassingMessage>(message =>
            {
                IsCurrentLockstepPeer = true;
            });

            MessageRegistry.RegisterMessage<SetClientIdMessage>(message =>
            {
                var idMesg = (SetClientIdMessage)message;
                PeerId = idMesg.id;
            });

            MessageRegistry.RegisterMessage<DisconnectMessage>(_ =>
            {
                Ready = false;
            });
        }

        /// <summary>
        /// Is the peer ready?
        /// </summary>
        public bool Ready { get; private set; } = true;

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
        /// Passes the lockstep simulation ownership to the other client.
        /// </summary>
        private void PassLockstep()
        {
            if (!IsCurrentLockstepPeer)
                return;

            Send(new LockstepPassingMessage(), passLockstep: false);
            IsCurrentLockstepPeer = false;
        }

        /// <summary>
        /// Send a network message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        /// <param name="mode">The mode of sending (either lockstep, or extra info).</param>
        /// <param name="passLockstep">Should we pass the lockstep ownership after sending this?</param>
        public void Send<TMessage>(TMessage message, SendMode mode = SendMode.Lockstep, bool passLockstep = true)
            where TMessage : INetworkMessage
        {
            if (mode == SendMode.Lockstep)
            {
                if (!IsCurrentLockstepPeer)
                    return;
            }

            var id = typeof(TMessage).GetHashCode();
            if (!MessageRegistry.HasMessageTypeId(id))
                return;

            var packet = new NetworkPacket(message, mode);
            var writer = new NetworkWriter(_buffer.Span);
            var written = packet.Serialize(ref writer);

            SendBytes(_buffer.Span[..written]);

            Console.WriteLine($"sent {written} bytes for message of type {typeof(TMessage).FullName}.");

            if (mode == SendMode.Lockstep && passLockstep)
                PassLockstep();
        }

        /// <summary>
        /// Receives all the currently pending messages.
        /// </summary>
        public void ReceiveMessages()
        {
            while (true)
            {
                var read = ReceiveBytes(_buffer);
                if (read == 0)
                    break;

                // For as long as we have data, try to read packets from it.
                var reader = new NetworkReader(_buffer.Span);
                while (reader.Position < read)
                {
                    var packet = new NetworkPacket(ref reader, MessageRegistry);

                    // If we have a message, try to handle it.
                    if (packet.Message != null &&
                        MessageRegistry.TryGetMessageHandler(packet.MessageType, out var handler))
                    {
                        handler!(packet.Message);
                    }
                }
            }
        }
    }
}
