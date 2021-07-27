using BinReader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BinReader
{
    public class VehModsReader
    {
        const ushort FILE_MODS_VERSION = 1;

        readonly List<ModKit> modKits = new();

        public VehModsReader()
        {
        }

        public List<ModKit> Read(string filePath)
        {
            using var fileStream = File.OpenRead(filePath);
            using var reader = new BinaryReader(fileStream);

            string versionString = reader.ReadChar().ToString();
            versionString += reader.ReadChar();
            var versionNumber = reader.ReadUInt16();

            if (versionString != "MO")
            {
                Console.WriteLine("Invalid vehmods.bin data file. Download new from altv.mp");
                return default;
            }

            if (versionNumber != FILE_MODS_VERSION)
            {
                Console.WriteLine($"vehmods.bin structure (v.{versionNumber}) doesn't fit server structure (v.{FILE_MODS_VERSION})");
                return default;
            }

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var modKit = new ModKit
                {
                    Id = reader.ReadUInt16(),
                    Name = new string(reader.ReadChars(reader.ReadUInt16())),
                    Mods = new Dictionary<byte, ushort[]>()
                };

                var modCount = reader.ReadByte();

                for (var i = 0; i < modCount; i++)
                {
                    var modCat = reader.ReadByte();
                    var count = reader.ReadByte();

                    modKit.Mods.TryGetValue(modCat, out var components);

                    var comps = components == null ? new List<ushort>() : components.ToList();

                    for (var j = 0; j < count; j++)
                    {
                        comps.Add(reader.ReadUInt16());
                    }

                    if (!modKit.Mods.TryAdd(modCat, comps.ToArray()))
                        modKit.Mods[modCat] = comps.ToArray();
                }

                modKits.Add(modKit);
                Console.WriteLine($"ModKit {modKit.Id} - {modKit.Name} - Count: {modKit.Mods.Count}");
            }

            return modKits;
        }

        public void Write(string outputName, ModKit[] modKits)
        {
            var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);

            writer.Write('M');
            writer.Write('O');

            writer.Write((ushort)1);

            foreach (var modKit in modKits.OrderBy(x => x.Id))
            {
                writer.Write(modKit.Id);
                writer.Write((ushort)modKit.Name.Length);

                foreach (var letter in modKit.Name)
                    writer.Write(letter);

                writer.Write((byte)modKit.Mods.Count);

                foreach(var mod in modKit.Mods.OrderBy(x => x.Key))
                {
                    writer.Write(mod.Key);
                    writer.Write((byte)mod.Value.Length);

                    foreach (var comp in mod.Value)
                        writer.Write(comp);
                }
            }

            File.Delete(outputName);
            using var fileStream = File.Open(outputName, FileMode.CreateNew);
            memoryStream.WriteTo(fileStream);

            writer.Dispose();
        }
    }
}
