using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleships.Framework.Networking
{
    /// <summary>
    /// The sending mode for a packet.
    /// </summary>
    internal enum SendMode
    {
        Lockstep,
        Extra
    }
}
