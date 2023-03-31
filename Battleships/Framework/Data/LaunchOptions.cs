using CommandLine;

namespace Battleships.Framework.Data
{
    /// <summary>
    /// The launch parameters for battleships.
    /// </summary>
    internal class LaunchOptions
    {
        [Option('m', "mode", HelpText = "The mode to launch in (server/client).", Default = "client")]
        public string Mode { get; set; } = null!;

        [Option('i', "ip", HelpText = "The IP address of the other peer.", Default = "127.0.0.1")]
        public string Ip { get; set; } = null!;

        [Option('p', "port", HelpText = "The port of the other peer.", Default = 666)]
        public int Port { get; set; }
    }
}
