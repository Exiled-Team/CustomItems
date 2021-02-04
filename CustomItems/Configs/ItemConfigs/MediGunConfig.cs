using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class MediGunConfig
    {
        public int ZombieHealingRequired { get; set; } = 200;
        public int ClipSize { get; set; } = 30;
        public ItemType ItemType { get; set; } = ItemType.GunProject90;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        public bool HealZombies { get; set; } = true;
        public float HealingModifier { get; set; } = 1f;
    }
}