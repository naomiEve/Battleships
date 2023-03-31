namespace Battleships.Framework.Networking
{
    /// <summary>
    /// The class containing all of the registered messages.
    /// </summary>
    internal class MessageRegistry
    {
        /// <summary>
        /// The metadata for the messages.
        /// </summary>
        private class MessageMetadata
        {
            /// <summary>
            /// The constructor for this message.
            /// </summary>
            public Func<INetworkMessage> Constructor { get; init; }

            /// <summary>
            /// The handler for this message.
            /// </summary>
            public Action<INetworkMessage> Handler { get; init; }

            /// <summary>
            /// Constructs a new instance of message metadata.
            /// </summary>
            /// <param name="constructor">The constructor for this INetworkMessage.</param>
            /// <param name="handler">The handler for this INetworkMessage.</param>
            internal MessageMetadata(Func<INetworkMessage> constructor, Action<INetworkMessage> handler)
            {
                Constructor = constructor;
                Handler = handler;
            }
        }

        /// <summary>
        /// The map from message numbers to their types. 
        /// </summary>
        private readonly Dictionary<int, MessageMetadata> _messageMetadataMap;

        /// <summary>
        /// Constructs a new message registry.
        /// </summary>
        public MessageRegistry()
        {
            _messageMetadataMap = new Dictionary<int, MessageMetadata>();
        }

        /// <summary>
        /// Returns whether we have this message type id.
        /// </summary>
        /// <param name="type">The type id of the message.</param>
        /// <returns>True if we have it, false otherwise.</returns>
        public bool HasMessageTypeId(int type)
        {
            return _messageMetadataMap.ContainsKey(type);
        }

        /// <summary>
        /// Tries to get the constructor of a message by its typeid.
        /// </summary>
        /// <param name="typeId">The type id.</param>
        /// <param name="constructor">The resulting constructor.</param>
        /// <returns>True if the constructor was found, false otherwise.</returns>
        public bool TryGetMessageConstructor(int typeId, out Func<INetworkMessage>? constructor)
        {
            if (_messageMetadataMap.TryGetValue(typeId, out var metadata))
            {
                constructor = metadata.Constructor;
                return true;
            }

            constructor = null;
            return false;
        }

        /// <summary>
        /// Tries to get the handler of a message by its typeid.
        /// </summary>
        /// <param name="typeId">The type id of the message.</param>
        /// <param name="handler">The resulting handler.</param>
        /// <returns>True if the handler was found, false otherwise.</returns>
        public bool TryGetMessageHandler(int typeId, out Action<INetworkMessage>? handler)
        {
            if (_messageMetadataMap.TryGetValue(typeId, out var metadata))
            {
                handler = metadata.Handler;
                return true;
            }

            handler = null;
            return false;
        }

        /// <summary>
        /// Register a network message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="handler">The handler to be invoked after we receive this message.</param>
        public void RegisterMessage<TMessage>(Action<INetworkMessage> handler)
            where TMessage : INetworkMessage, new()
        {
            var metadata = new MessageMetadata(() => ConstructMessageFromType<TMessage>(), handler);

            _messageMetadataMap.Add(typeof(TMessage).GetHashCode(), metadata);
            Console.WriteLine($"Registered {typeof(TMessage).FullName}=>{typeof(TMessage).GetHashCode()}");
        }

        /// <summary>
        /// Constructs a new instance of a message of type TMessage.
        /// </summary>
        /// <typeparam name="TMessage">The message type, deriving from INetworkMessage.</typeparam>
        /// <returns>The newly created message.</returns>
        private static TMessage ConstructMessageFromType<TMessage>()
            where TMessage : INetworkMessage, new()
        {
            return new TMessage();
        }
    }
}
