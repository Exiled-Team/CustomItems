using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class MediGunConfig
    {
        [Description("The amount of total 'healing' a zombie will require before being cured.")]
        public int ZombieHealingRequired { get; set; } = 200;
        
        [Description("How many rounds are usable after a single reload.")]
        public int ClipSize { get; set; } = 30;
        
        [Description("The base weapon this one will be modeled after.")]
        public ItemType ItemType { get; set; } = ItemType.GunProject90;

        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        
        [Description("Whether or not zombies can be 'cured' by this weapon.")]
        public bool HealZombies { get; set; } = true;
        
        [Description("The % of damage the weapon would normally deal, that is converted into healing. 1 = 100%, 0.5 = 50%, 0.0 = 0%")]
        public float HealingModifier { get; set; } = 1f;
    }
}