using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class GrenadeLauncherConfig
    {
        public bool UseGrenades { get; set; } = true;
        public int ClipSize { get; set; } = 1;
        public ItemType ItemType { get; set; } = ItemType.GunLogicer;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        public float GrenadeSpeed { get; set; } = 1f;
        public float FuseTime { get; set; } = 1f;
    }
}