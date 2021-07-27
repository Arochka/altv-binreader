using BinReader.Enums;
using BinReader.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BinReader
{
    public class CarcolsReader
    {
        public CarcolsReader()
        {

        }

        public IEnumerable<ModKit> Read(string directory)
        {
            var files = Directory.EnumerateFiles(directory);

            var modKits = new List<ModKit>();

            foreach (var file in files)
            {
                var doc = new XmlDocument();
                doc.Load(file);

                var kits = doc.SelectNodes("/CVehicleModelInfoVarGlobal/Kits/Item");

                foreach (XmlElement kit in kits)
                {
                    var modKit = new ModKit
                    {
                        Id = ushort.Parse(kit.GetElementsByTagName("id")[0].Attributes[0].InnerText),
                        Name = kit.GetElementsByTagName("kitName")[0].InnerText,
                        Mods = new Dictionary<byte, ushort[]>()
                    };

                    ushort lastIndex = 0;

                    var visibleMods = kit.SelectNodes("visibleMods/Item");
                    var statsMods = kit.SelectNodes("statMods/Item");

                    var modTypes = new Dictionary<ushort, VehicleModType>();

                    foreach (XmlElement visibleMod in visibleMods)
                    {
                        var type = visibleMod.GetElementsByTagName("type")[0].InnerText;
                        modTypes.Add(lastIndex, GetModType(type));
                        lastIndex++;
                    }

                    foreach (XmlElement statMod in statsMods)
                    {
                        var type = statMod.GetElementsByTagName("type")[0].InnerText;
                        modTypes.Add(lastIndex, GetModType(type));
                        lastIndex++;
                    }

                    foreach (var modType in modTypes.Where(x => x.Value != VehicleModType.UNKNOWN).GroupBy(x => x.Value))
                    {
                        modKit.Mods.TryAdd((byte)modType.Key, modType.Select(x => x.Key).ToArray());
                    }

                    modKits.Add(modKit);
                }
            }

            foreach (var dupModkits in modKits.GroupBy(x => x.Name).Where(x => x.Count() > 1))
            {
                var mostCompleteModKit = dupModkits.OrderBy(x => x.Mods.Values.Sum(y => y.Count())).LastOrDefault();
                modKits.RemoveAll(x => x.Name == dupModkits.Key && x != mostCompleteModKit);
            }

            return modKits;
        }

        VehicleModType GetModType(string type)
        {
            if (!Enum.TryParse<VehicleModType>(type, out var modType))
                return VehicleModType.UNKNOWN;

            return modType;
        }

        public T DeserializeToObject<T>(string filepath) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StreamReader sr = new StreamReader(filepath))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
