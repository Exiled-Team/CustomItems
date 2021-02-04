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

        public bool ExplodeOnCollision { get; set; } = true;
        public float FuseDuration { get; set; } = 3f;
    }
}