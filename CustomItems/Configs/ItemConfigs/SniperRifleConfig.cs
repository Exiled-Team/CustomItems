using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class SniperRifleConfig
    {
        [Description("The amoutn of extra damage this weapon does, as a multiplier.")]
        public float DamageMultiplier { get; set; } = 7.5f;
        
        [Description("How many rounds are usable after a single reload.")]
        public int ClipSize { get; set; } = 1;
        
        [Description("The base weapon this one is modeled after.")]
        public ItemType ItemType { get; set; } = ItemType.GunE11SR;
        
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();

        [Description("The Custom Item ID for this item.")]
        public int Id { get; set; } = 9;
        
        [Description("The description of this item show to players when they obtain it.")]
        public string Description { get; set; } = "This modified E-11 Rifle fires high-velocity anti-personnel sniper rounds.";

        [Description("The name of this item shown to players when they obtain it.")]
        public string Name { get; set; } = "SR-119";
    }
}