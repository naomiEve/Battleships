namespace Battleships.Framework.Networking
{
    /// <summary>
    /// An abstract network peer, common class for both the sevrer and client.
    /// </summary>
    internal abstract class NetworkPeer
    {
        /// <summary>
        /// Is the peer ready?
        /// </summary>
        public abstract bool Ready { get; }

        /// <summary>
        /// Waits until this peer is ready to work.
        /// </summary>
        public abstract void WaitUntilReady();
    }
}
