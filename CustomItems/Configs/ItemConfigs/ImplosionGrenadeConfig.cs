using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class ImplosionGrenadeConfig
    {
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        
        [Description("The % of normal frag grenade damage this grenade will deal to those in it's radius.")]
        public float DamageModifier { get; set; } = 0.1f;
        
        [Description("The amount of suction ticks each grenade will generate.")]
        public int SuctionCount { get; set; } = 90;
        
        [Description("The distance each tick will move players towards the center.")]
        public float SuctionPerTick { get; set; } = 0.125f;
        
        [Description("How often each suction tick will occus. Note: Setting the tick-rate and suction-per-tick to lower numbers maks for a 'smoother' suction movement, however causes more stress on your server. Adjust accordingly.")]
        public float SuctionTickRate { get; set; } = 0.025f;
    }
}