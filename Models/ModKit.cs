using System.Collections.Generic;

namespace BinReader.Models
{
    public class ModKit
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
        public Dictionary<byte, ushort[]> Mods { get; set; }
    }
}
