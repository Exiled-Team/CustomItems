using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class TranqGunConfig
    {
        public int ClipSize { get; set; } = 2;
        public ItemType ItemType { get; set; } = ItemType.GunUSP;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        public bool ResistantScps { get; set; } = true;
        public float Duration { get; set; } = 5f;
        public int ResistanceModifier { get; set; } = 2;
        public bool DropItems { get; set; } = true;
        public int ScpResistChance { get; set; } = 40;
    }
}