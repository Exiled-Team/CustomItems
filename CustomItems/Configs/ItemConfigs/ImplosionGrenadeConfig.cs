using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class ImplosionGrenadeConfig
    {
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        public float DamageModifier { get; set; } = 0.9f;
        public int SuctionCount { get; set; } = 90;
        public float SuctionPerTick { get; set; } = 0.125f;
        public float SuctionTickRate { get; set; } = 0.025f;
    }
}