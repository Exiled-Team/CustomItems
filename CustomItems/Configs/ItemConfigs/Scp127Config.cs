using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class Scp127Config
    {
        public float RegenDelay { get; set; } = 10f;
        public int RegenAmount { get; set; } = 2;
        public int ClipSize { get; set; } = 12;
        public ItemType ItemType { get; set; } = ItemType.GunCOM15;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
    }
}