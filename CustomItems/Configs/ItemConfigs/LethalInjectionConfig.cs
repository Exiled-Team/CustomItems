using System.Collections.Generic;
using System.ComponentModel;
using CustomItems.API;

namespace CustomItems.ItemConfigs
{
    public class LethalInjectionConfig
    {
        [Description("Where on the map items should spawn, and their % chance of spawning in each location.")]
        public Dictionary<SpawnLocation, float> SpawnLocations { get; set; } = new Dictionary<SpawnLocation, float>();
        
        [Description("Whether the Lethal Injection shoudl always kill the user, regardless of if they stop SCP-096's enrage.")]
        public bool KillOnFail { get; set; } = true;
    }
}