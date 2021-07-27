namespace BinReader.Models
{
    public class Mod
    {
        public int Index { get; set; }
        public int Type { get; set; }
        public string Bone { get; set; }
        public string CollisionBone { get; set; }
        public int Weight { get; set; }
        public bool IsWeapon { get; set; }
    }

    public class VehicleModKit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Mod[] Mods { get; set; }
        public string[] VehicleModels { get; set; }
    }
}
