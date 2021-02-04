using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class ShotgunConfig
    {
        public int SpreadCount { get; set; } = 12;
        public int AimconeSeverity { get; set; } = 5;
        public float BaseDamage { get; set; } = 13.5f;
        public ItemType ItemType { get; set; } = ItemType.GunMP7;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        public float DamageFalloffModifier { get; set; } = 0.9f;
    }
}