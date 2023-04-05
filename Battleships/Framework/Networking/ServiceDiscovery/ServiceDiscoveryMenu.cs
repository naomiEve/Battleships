namespace Battleships.Framework.Networking.ServiceDiscovery
{
    /// <summary>
    /// Used before the main game starts, to pick the game we're connecting to.
    /// </summary>
    internal class ServiceDiscoveryMenu
    {
        /// <summary>
        /// The client that's receiving the discovered services.
        /// </summary>
        private readonly ServiceDiscoveryClient _client;

        /// <summary>
        /// Constructs a new discovery menu.
        /// </summary>
        public ServiceDiscoveryMenu()
        {
            _client = new ServiceDiscoveryClient();
        }

        /// <summary>
        /// Shows the menu and returns the ip/port pair.
        /// </summary>
        /// <returns>The ip/port pair.</returns>
        public async Task<(string, int)> ShowMenu()
        {
            _client.StartListeningForServices();

            Console.Clear();

            ServiceInfo outInfo;
            while (true)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;

                Console.WriteLine("Available services: ");
                lock (_client.Services)
                {
                    for (var i = 0; i < _client.Services.Count; i++)
                        Console.WriteLine($"{i + 1}. {_client.Services[i].Hostname}'s game ({_client.Services[i].Ip}:{_client.Services[i].Port})");
                }

                Console.Write('>');
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().KeyChar;
                    var value = key - '0';

                    if (value < 1 || value > 9)
                        continue;

                    // We have a proper index, pick it.
                    lock (_client.Services)
                        outInfo = _client.Services[value - 1];
                    break;
                }

                await Task.Delay(1000/60);
            }

            _client.StopListeningForServices();

            return (outInfo.Ip!, outInfo.Port);
        }
    }
}
