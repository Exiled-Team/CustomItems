using System.Collections.Generic;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class EmpGrenadeConfig
    {
        public float Duration { get; set; } = 20f;
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            {
                SpawnLocation.Inside173Gate, 100
            }
        };
    }
}