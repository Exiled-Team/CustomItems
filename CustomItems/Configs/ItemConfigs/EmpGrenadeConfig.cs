using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class EmpGrenadeConfig
    {
        [Description("How long the EMP effect should last on the rooms affected.")]
        public float Duration { get; set; } = 20f;
        
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>
        {
            {
                SpawnLocation.Inside173Gate, 100
            }
        };

        [Description("Whether or not the grenade should explode immediately when it collides with something.")]
        public bool ExplodeOnCollision { get; set; } = true;
        
        [Description("How long the fuse duration should be. Has no effect if ExplodeOnCollision is true.")]
        public float FuseDuration { get; set; } = 3f;
    }
}