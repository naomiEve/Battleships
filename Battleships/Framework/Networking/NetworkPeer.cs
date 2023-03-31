namespace Battleships.Framework.Networking
{
    /// <summary>
    /// An abstract network peer, common class for both the sevrer and client.
    /// </summary>
    internal abstract class NetworkPeer
    {
        /// <summary>
        /// The map from message numbers to their types. 
        /// </summary>
        private readonly Dictionary<int, Type> _messageTypeMap;

        /// <summary>
        /// Constructs a new network peer.
        /// </summary>
        public NetworkPeer()
        {
            _messageTypeMap = new Dictionary<int, Type>();
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
        /// Register a network message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        public void RegisterMessage<TMessage>()
            where TMessage : INetworkMessage
        {
            _messageTypeMap.Add(typeof(TMessage).GetHashCode(), typeof(TMessage));
            Console.WriteLine($"Registered {typeof(TMessage).FullName}=>{typeof(TMessage).GetHashCode()}");
        }
    }
}
