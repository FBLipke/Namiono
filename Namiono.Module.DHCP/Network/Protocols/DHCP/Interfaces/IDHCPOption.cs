using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Namiono.Module.Network.Protocols.DHCP
{
    public interface IDHCPOption
    {
        byte Option { get; }

        byte Length { get; }

        byte[] Data { get; }
    }
}
